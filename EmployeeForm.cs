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

namespace FacialRecognition
{
    public partial class EmployeeForm: Form
    {

        // private string connectionString = "your_connection_string_here"; // Replace with your database connection string
        string connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                  $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                  $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                  $"Password={Environment.GetEnvironmentVariable("DB_PASS")};" +
                                  $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                                  $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERT")};";


        public EmployeeForm()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();
            LoadEmployees();

        }

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
            }
            ;
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
                    NpgsqlCommand insertCmd;

                    insertQuery = @"INSERT INTO employees (emp_id, rid, first_name, last_name, email, phone, password, facial_path, qr_code_path)
                                    SELECT pending_emp_id, rid, first_name, last_name, email, phone, password, facial_path, qr_code_path
                                    FROM pending_employees WHERE pending_emp_id = @pending_emp_id";

                    insertCmd = new NpgsqlCommand(insertQuery, conn, transaction);
                    insertCmd.Parameters.AddWithValue("@pending_emp_id", id);
                    insertCmd.ExecuteNonQuery();

                    // Delete the user from pending_employees Table
                    string deleteQuery = "DELETE FROM pending_employees WHERE pending_emp_id = @pending_emp_id";
                    NpgsqlCommand deleteCmd = new NpgsqlCommand(deleteQuery, conn, transaction);
                    deleteCmd.Parameters.AddWithValue("@rid", id);
                    deleteCmd.ExecuteNonQuery();

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
