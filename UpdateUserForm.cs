using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace ManageStoredUsersForm.cs
{
    public partial class UpdateUserForm : Form
    {
        public UpdateUserForm(string userType, int userId, string rid, string name, string surname, string email,
                                string password, string phoneNumber, string qrCode, string imagePath)
        {
            InitializeComponent();
            DotNetEnv.Env.Load();

            _userType = userType.ToLower(); // "employee" or "visitor"
            _userId = userId;
            _qrCode = qrCode;
            _imagePath = imagePath;

            cmbStatus.Items.Add("employee");
            cmbStatus.Items.Add("visitor");
            cmbStatus.SelectedItem = _userType;

            txtRID.Text = rid;
            txtName.Text = name;
            txtSurname.Text = surname;
            txtEmail.Text = email;
            txtPassword.Text = password;
            txtPhoneNumber.Text = phoneNumber;

            UpdateFieldVisibility();
        }


        private readonly int _userId;
        private readonly string _userType; // "employee" or "visitor"

        private readonly string _connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_PASS")};" +
                               $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                               $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERT")};";


        private readonly string _qrCode;
        private readonly string _imagePath;

      

        private void UpdateFieldVisibility()
        {
            bool isEmployee = _userType == "employee";
            lblPassword.Visible = isEmployee;
            txtPassword.Visible = isEmployee;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtSurname.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string query;
                    var command = new NpgsqlCommand();
                    command.Connection = connection;

                    if (_userType == "employee")
                    {
                        query = @"
                            UPDATE employees SET 
                                rid = @rid,
                                first_name = @first_name,
                                last_name = @last_name,
                                email = @email,
                                phone = @phone,
                                password = @password
                            WHERE emp_id = @id";

                        command.CommandText = query;
                        command.Parameters.AddWithValue("@password", txtPassword.Text);
                    }
                    else // visitor
                    {
                        query = @"
                            UPDATE visitors SET 
                                rid = @rid,
                                first_name = @first_name,
                                last_name = @last_name,
                                email = @email,
                                phone = @phone
                            WHERE visitor_id = @id";

                        command.CommandText = query;
                    }

                    // Common parameters
                    command.Parameters.AddWithValue("@rid", txtRID.Text);
                    command.Parameters.AddWithValue("@first_name", txtName.Text);
                    command.Parameters.AddWithValue("@last_name", txtSurname.Text);
                    command.Parameters.AddWithValue("@email", txtEmail.Text);
                    command.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text);
                    command.Parameters.AddWithValue("@id", _userId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("No changes were made.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void UpdateUserForm_Load(object sender, EventArgs e)
        {
            // Optional: add startup behavior here
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: dynamically change visibility if status is made editable
            UpdateFieldVisibility();
        }
    }
}