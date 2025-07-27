using Npgsql;
using QRCoder;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Windows.Forms;
using static QRCoder.PayloadGenerator;



namespace FacialRecognition
{
    public partial class VisitorForm: Form
    {
        public VisitorForm()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();
            LoadVisitors();
        }

        private string connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                  $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                  $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                  $"Password={Environment.GetEnvironmentVariable("DB_PASS")};" +
                                  $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                                  $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERT")};";



        private void LoadVisitors()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sqlQuery;
                NpgsqlDataAdapter da;
                DataTable dt;

                sqlQuery = "SELECT * FROM pending_visitors";
                da = new NpgsqlDataAdapter(sqlQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                dgvVisitors.DataSource = dt;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (dgvVisitors.CurrentRow != null)
            {
                int id;

                id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["pending_visitor_id"].Value);
                AcceptUser(id);
                
            }
            ;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // Open the Main Menu Form form (Raza,2021)
            MainMenuForm mm;
            mm = new MainMenuForm();
            mm.Show();
            this.Hide(); // Hide Admin Form
        }

        private void AcceptUser(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                NpgsqlTransaction transaction = conn.BeginTransaction();  // Start transaction

                try
                {
                    // Insert selected user into visitors table
                    string insertQuery;
                    string qrCodeBase64;
                    string email_subject = "Approved Visitor";
                    Image qrCodeVisitorImage;

                    string selectQuery = @"SELECT rid, first_name, last_name, email, phone, registration_date FROM pending_visitors WHERE pending_visitor_id = @id";
                    string qrCodeDataString;

                    string rid = "", firstName = "", lastName = "", email = "", phone = "";

                    using (NpgsqlCommand selectCmd = new NpgsqlCommand(selectQuery, conn, transaction))
                    {
                        selectCmd.Parameters.AddWithValue("@id", id);
                        using (var reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                rid = reader["rid"].ToString();
                                firstName = reader["first_name"].ToString();
                                lastName = reader["last_name"].ToString();
                                email = reader["email"].ToString();
                                phone = reader["phone"].ToString();
                            }
                            else
                            {
                                throw new Exception("Employee not found.");
                            }
                        }
                    }

                    // 2. Create QR content from the retrieved data
                    qrCodeDataString = $"ID: {id}\nRID: {rid}\nName: {firstName} {lastName}\nEmail: {email}\nPhone: {phone}";

                    // 3. Generate QR code
                    using (QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())
                    using (QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(qrCodeDataString, QRCodeGenerator.ECCLevel.Q))
                    using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeImage = qrCode.GetGraphic(20);
                        qrCodeBase64 = Convert.ToBase64String(qrCodeImage);
                    }
                    

                    byte[] imageBytes = Convert.FromBase64String(qrCodeBase64);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        qrCodeVisitorImage = Image.FromStream(ms);
                    }


                    NpgsqlCommand insertCmd;

                    insertQuery = @"INSERT INTO visitors(visitor_id, rid, first_name, last_name, email, phone, qr_code_path)
                                   SELECT pending_visitor_id, rid, first_name, last_name, email, phone, @qr_code_path
                                   FROM pending_visitors WHERE pending_visitor_id = @pending_visitor_id";

                    insertCmd = new NpgsqlCommand(insertQuery, conn, transaction);
                    insertCmd.Parameters.AddWithValue("@pending_visitor_id", id);
                    insertCmd.Parameters.AddWithValue("@qr_code_path", qrCodeBase64);
                    insertCmd.ExecuteNonQuery();

                    // Delete the user from pending_visitors table
                    string deleteQuery;
                    NpgsqlCommand deleteCmd;

                    deleteQuery = "DELETE FROM pending_visitors WHERE pending_visitor_id = @pending_visitor_id";
                    deleteCmd = new NpgsqlCommand(deleteQuery, conn, transaction);
                    deleteCmd.Parameters.AddWithValue("@pending_visitor_id", id);
                    deleteCmd.ExecuteNonQuery();

                    MailMessage mail = new MailMessage();
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                    mail.From = new MailAddress(Environment.GetEnvironmentVariable("EMAIL_SENDER"));
                    mail.To.Add(email);  // Recipient from database
                    mail.Subject = email_subject;
                    mail.IsBodyHtml = true;

                    smtp.Credentials = new System.Net.NetworkCredential(
                        Environment.GetEnvironmentVariable("EMAIL_SENDER"),
                        Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                    );
                    smtp.EnableSsl = true;

                    // Convert Image to memory stream for attachment
                    MemoryStream imageStream = new MemoryStream();
                    qrCodeVisitorImage.Save(imageStream, ImageFormat.Png);
                    imageStream.Position = 0;

                    // Create the alternate view with embedded image
                    LinkedResource qrImage = new LinkedResource(imageStream, "image/png")
                    {
                        ContentId = "qrCodeImage",
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                    };

                    string htmlBody = $@"
                                        <html>
                                             <body style='font-family: Arial, sans-serif; font-size: 14px;'>
                                                <p>Dear {rid},</p>
                                                <p>Your visitor registration has been approved &#10003;.</p>
                                                <p>You can now access the premises using the QR code below: &#128247;</p>
                                                <img src='cid:qrCodeImage' />
                                                <p>Best regards,<br>Atos Interns</p>
                                            </body>
                                        </html>";

                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
                    avHtml.LinkedResources.Add(qrImage);
                    mail.AlternateViews.Add(avHtml);

                    smtp.Send(mail);


                    transaction.Commit(); // Commit transaction
                    MessageBox.Show("User Accepted and Moved Successfully!");
                    LoadVisitors(); // Refresh the grid
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback on failure
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void DeclineUser(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query;
                NpgsqlCommand cmd;

                query = "DELETE FROM pending_visitors WHERE pending_visitor_id = @pending_visitor_id";
                cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@pending_visitor_id", id);
                cmd.ExecuteNonQuery();
                LoadVisitors();
                MessageBox.Show("User Declined!");
            }
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            if (dgvVisitors.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["pending_visitor_id"].Value);
                DeclineUser(id);
            }

        }


    }
}

