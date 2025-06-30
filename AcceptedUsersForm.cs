using ManageStoredUsersForm.cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace FacialRecognition
{
    public partial class AcceptedUsersForm: Form
    {
        public AcceptedUsersForm()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();
            LoadAcceptedUsers();
        }

        private string connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                          $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                          $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                          $"Password={Environment.GetEnvironmentVariable("DB_PASS")};" +
                                          $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                                          $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERT")};";


        private void LoadAcceptedUsers()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Load Employees
                string employeeQuery;
                NpgsqlDataAdapter employeeAdapter;
                DataTable employeeTable;


                employeeQuery = "SELECT * FROM employees";
                employeeAdapter = new NpgsqlDataAdapter(employeeQuery, conn);
                employeeTable = new DataTable();
                employeeAdapter.Fill(employeeTable);
                dgvEmployee.DataSource = employeeTable;


                // Load Visitors
                string visitorQuery;
                NpgsqlDataAdapter visitorAdapter;
                DataTable visitorTable;

                visitorQuery = "SELECT * FROM visitors";
                visitorAdapter = new NpgsqlDataAdapter(visitorQuery, conn);
                visitorTable = new DataTable();
                visitorAdapter.Fill(visitorTable);
                dgvVisitors.DataSource = visitorTable;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvEmployee.CurrentRow.Cells["emp_id"].Value);
                UpdateUser(id);
            }
            else if (dgvVisitors.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["visitor_id"].Value);
                UpdateUser(id);
            }
        }

        private void UpdateUser(int id)
        {
            string newName;
            string newSurname;

            newName = Prompt.ShowDialog("Enter new name:", "Update User");
            newSurname = Prompt.ShowDialog("Enter new surname:", "Update User");

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query;
                NpgsqlCommand cmd;

                query = "UPDATE AcceptedTable SET Name = @Name, Surname = @Surname WHERE ID = @ID";
                cmd = new NpgsqlCommand(query, conn);
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
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM AcceptedTable WHERE ID = @ID";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
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

