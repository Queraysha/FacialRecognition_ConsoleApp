using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using Npgsql;
using QRCoder;
using System.Net.Mail;

namespace FacialRecognition
{
    public partial class EmployeeForm: Form
    {
        public EmployeeForm()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();
            LoadEmployees();
        }


        private string connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                          $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                          $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                          $"Password={Environment.GetEnvironmentVariable("DB_PASS")};" +
                                          $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                                          $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERT")};";



        private void LoadEmployees()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sqlQuery;
                NpgsqlDataAdapter da;
                DataTable dt;

                sqlQuery = "SELECT * FROM pending_employees";
                da       = new NpgsqlDataAdapter(sqlQuery, conn);
                dt       = new DataTable();
                da.Fill(dt);
                dgvEmployee.DataSource = dt;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null)
            {
                int id;
                id = Convert.ToInt32(dgvEmployee.CurrentRow.Cells["pending_emp_id"].Value);
                AcceptUser(id);
            };
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // Open the Main Menu Form form (Raza,2021)
            MainMenuForm mm = new MainMenuForm();
            mm.Show();
            this.Hide(); // Hide Admin Form
        }


        private void btnDecline_Click_1(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null)
            {
                int id;
                id = Convert.ToInt32(dgvEmployee.CurrentRow.Cells["pending_emp_id"].Value);
                DeclineUser(id);
            }
        }

        private void AcceptUser(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                NpgsqlTransaction transaction;
                transaction = conn.BeginTransaction();  // Start transaction

                try
                {
                    // Insert selected user into employees
                    string insertQuery;
                    string qrCodeBase64;
                    string email_subject = "Approved Employee";

                    string selectQuery = @"SELECT rid, first_name, last_name, email, phone, registration_date FROM pending_employees WHERE pending_emp_id = @id";
                    string qrCodeDataString;

                    // Decrypt values using Cryptography.Fernet
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

                    NpgsqlCommand insertCmd;

                    insertQuery = @"INSERT INTO employees (emp_id, rid, first_name, last_name, email, phone, user_password, facial_path, qr_code_path)
                                    SELECT pending_emp_id, rid, first_name, last_name, email, phone, user_password, facial_path, @qr_code_path
                                    FROM pending_employees WHERE pending_emp_id = @pending_emp_id";

                    insertCmd = new NpgsqlCommand(insertQuery, conn, transaction);
                    insertCmd.Parameters.AddWithValue("@pending_emp_id", id);
                    insertCmd.Parameters.AddWithValue("@qr_code_path", qrCodeBase64);
                    insertCmd.ExecuteNonQuery();

                    // Delete the user from pending_employees Table
                    string deleteQuery = "DELETE FROM pending_employees WHERE pending_emp_id = @pending_emp_id";
                    NpgsqlCommand deleteCmd = new NpgsqlCommand(deleteQuery, conn, transaction);
                    deleteCmd.Parameters.AddWithValue("@pending_emp_id", id);
                    deleteCmd.ExecuteNonQuery();


                    MailMessage mail = new MailMessage();
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                    mail.From = new MailAddress(Environment.GetEnvironmentVariable("EMAIL_SENDER"));
                    mail.To.Add(email);
                    mail.Subject = email_subject;
                    mail.IsBodyHtml = true;

                    smtp.Credentials = new System.Net.NetworkCredential(
                        Environment.GetEnvironmentVariable("EMAIL_SENDER"),
                        Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                    );
                    smtp.EnableSsl = true;

                    // HTML body with tags and emojis
                    string htmlBody = $@"
                                        <html>
                                          <body style='font-family: Arial, sans-serif; font-size: 14px;'>
                                            <p>Dear {rid},<br><br>
                                            Your employee registration has been approved. &#10003;<br>
                                            You can now access the system using your credentials.<br>
                                            Please wait 2 minutes &#9203; after receiving this email to sign in...<br>
                                            Click to Sign in:<a href='https://facialrecognitionwebsite.onrender.com/'>Sign in</a><br><br>
                                            Best regards,<br>
                                            Atos Interns</p>
                                          </body>
                                        </html>";

                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
                    mail.AlternateViews.Add(avHtml);
                    smtp.Send(mail);

                    transaction.Commit(); // Commit transaction
                    MessageBox.Show("User Accepted and Moved Successfully!");
                    LoadEmployees(); // Refresh the grid
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
                string query = "DELETE FROM pending_employees WHERE pending_emp_id = @pending_emp_id";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@pending_emp_id", id);
                cmd.ExecuteNonQuery();
                LoadEmployees();
                MessageBox.Show("User Declined!");
            }
        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {

        }
    }
}
