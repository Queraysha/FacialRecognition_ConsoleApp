using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManageStoredUsersForm.cs
{
    public partial class UpdateUserForm : Form
    {
        private int _userId;
        private string _connectionString = "Data Source=lindiwe;Initial Catalog=YourDatabase;Integrated Security=True";
        private string _qrCode;
        private string _imagePath;

        public UpdateUserForm(int userId, string rid, string status, string name, string surname, string email,
                             string password, DateTime? entryDate, string phoneNumber, string qrCode, string imagePath)
        {
            InitializeComponent();

            _userId = userId;
            _qrCode = qrCode;
            _imagePath = imagePath;

            // Populate the status (vis/emp) dropdown
            cmbStatus.Items.Add("vis");
            cmbStatus.Items.Add("emp");

            // Set the current status
            cmbStatus.SelectedItem = status;

            // Populate form fields with user data


            txtRID.Text = rid;
            txtName.Text = name;
            txtSurname.Text = surname;
            txtEmail.Text = email;
            txtPassword.Text = password;
            txtPhoneNumber.Text = phoneNumber;

            if (entryDate.HasValue)
            {
                dtpEntryDate.MaxDate = entryDate.Value;
            }
            else
            {
                dtpEntryDate.MaxDate = DateTime.Now;
            }

            // Update field visibility based on status (vis/emp)
            UpdateFieldVisibility();
        }

        private void UpdateFieldVisibility()
        {
            string status = cmbStatus.SelectedItem?.ToString();

            // Password only for employees
            lblPassword.Visible = status == "emp";
            txtPassword.Visible = status == "emp";

            // Entry date only for visitors
            lblEntryDate.Visible = status == "vis";
            dtpEntryDate.Visible = status == "vis";
            dtpEntryDate.Visible = status == "vis"; // Show calendar only for visitors
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFieldVisibility();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (
                    string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtSurname.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Additional validation for employees (must have password)
                if (cmbStatus.SelectedItem.ToString() == "emp" && string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Password is required for employees.", "Validation Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"UPDATE StoredUsers SET 
                                     RID = @RID, 
                                    Status = @Status, 
                                    Name = @Name, 
                                    Surname = @Surname, 
                                    Email = @Email, 
                                    Password = @Password, 
                                    EntryDate = @EntryDate,
                                    PhoneNumber = @PhoneNumber
                                    WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RID", txtRID.Text);
                        command.Parameters.AddWithValue("@Status", cmbStatus.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Name", txtName.Text);
                        command.Parameters.AddWithValue("@Surname", txtSurname.Text);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);

                        // Handle conditional fields
                        if (cmbStatus.SelectedItem.ToString() == "emp")
                        {
                            command.Parameters.AddWithValue("@Password", txtPassword.Text);
                            command.Parameters.AddWithValue("@EntryDate", DBNull.Value);
                        }
                        else // visitor
                        {
                            command.Parameters.AddWithValue("@Password", DBNull.Value);
                            command.Parameters.AddWithValue("@EntryDate", dtpEntryDate.MaxDate);
                        }

                        // Handle optional field
                        if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
                        {
                            command.Parameters.AddWithValue("@PhoneNumber", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text);
                        }

                        command.Parameters.AddWithValue("@Id", _userId);

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
                            MessageBox.Show("No changes were made.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
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

        private void dtpEntryDate_DateChanged(object sender, DateRangeEventArgs e)
        {
            // This code will run when the user selects a different date
            DateTime selectedDate = dtpEntryDate.SelectionStart;
            MessageBox.Show($"Selected date: {selectedDate.ToShortDateString()}");

            dtpEntryDate.Visible = !dtpEntryDate.Visible;
            if (dtpEntryDate.Visible)
            {
                // Position the calendar near the date time picker if needed
                dtpEntryDate.Location = new Point(dtpEntryDate.Location.X,
                                                  dtpEntryDate.Location.Y + dtpEntryDate.Height);
            }




        }
    }
}