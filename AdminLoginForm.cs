﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace FacialRecognition
{
    public partial class AdminLoginForm: Form
    {
        public AdminLoginForm()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            string username;
            string password;

            username = txtUsername.Text.Trim();
            password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ValidateUser(username, password))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Open the Main Menu Form form (Raza,2021)
                MainMenuForm mm;

                mm = new MainMenuForm();
                mm.Show();
                this.Hide(); // Hide Admin Form
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateUser(string username, string password)
        {
            string connectionString;
            string sqlQuery;
            
            connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_PASS")};" +
                               $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                               $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERT")};";

           
            sqlQuery = "SELECT * FROM admins " +
                       "WHERE username = @username AND password = @password";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn))
                    {
                        int count;

                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        count = (int)cmd.ExecuteScalar();
                        return count == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void AdminLoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
