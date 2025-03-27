using ManageStoredUsersForm.cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacialRecognition
{
    public partial class AcceptedUsersForm: Form
    {

        private string connectionString = @"Data Source= LAPTOP-EDM9E5LN;Initial Catalog=VisitorManagementDB;Integrated Security=True";


        public AcceptedUsersForm()
        {
            InitializeComponent();
            LoadAcceptedUsers();
        }
        private void LoadAcceptedUsers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Load Employees
                string employeeQuery = "SELECT * FROM AcceptedTable WHERE Status = 'Employee'";
                SqlDataAdapter employeeAdapter = new SqlDataAdapter(employeeQuery, conn);
                DataTable employeeTable = new DataTable();
                employeeAdapter.Fill(employeeTable);
                dgvEmployee.DataSource = employeeTable;

                // Load Visitors
                string visitorQuery = "SELECT * FROM AcceptedTable WHERE Status = 'Visitor'";
                SqlDataAdapter visitorAdapter = new SqlDataAdapter(visitorQuery, conn);
                DataTable visitorTable = new DataTable();
                visitorAdapter.Fill(visitorTable);
                dgvVisitors.DataSource = visitorTable;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvEmployee.CurrentRow.Cells["ID"].Value);
                UpdateUser(id);
            }
            else if (dgvVisitors.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["ID"].Value);
                UpdateUser(id);
            }
        }

        private void UpdateUser(int id)
        {
            string newName = Prompt.ShowDialog("Enter new name:", "Update User");
            string newSurname = Prompt.ShowDialog("Enter new surname:", "Update User");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE AcceptedTable SET Name = @Name, Surname = @Surname WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", newName);
                cmd.Parameters.AddWithValue("@Surname", newSurname);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("User updated successfully!");
                LoadAcceptedUsers();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvEmployee.CurrentRow.Cells["ID"].Value);
                RemoveUser(id);
            }
            else if (dgvVisitors.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["ID"].Value);
                RemoveUser(id);
            }
        }

        private void RemoveUser(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM AcceptedTable WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("User removed successfully!");
                LoadAcceptedUsers();
            }
        }

    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                Text = caption
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox inputBox = new TextBox() { Left = 50, Top = 50, Width = 300 };
            Button confirmation = new Button() { Text = "OK", Left = 250, Width = 100, Top = 80 };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.ShowDialog();

            return inputBox.Text;
        }
    }
}

