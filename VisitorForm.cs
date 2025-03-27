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

namespace FacialRecognition
{
    public partial class VisitorForm: Form
    {

        string connectionString = @"Data Source= LAPTOP-EDM9E5LN;Initial Catalog=VisitorManagementDB;Integrated Security=True";

        public VisitorForm()
        {
            InitializeComponent();
            LoadVisitors();
        }

        private void LoadVisitors()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM RequestTable WHERE Status = 'Visitor'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvVisitors.DataSource = dt;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (dgvVisitors.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["ID"].Value);
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

        private void AcceptUser(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();  // Start transaction

                try
                {
                    // Insert selected user into AcceptedTable
                    string insertQuery = @"INSERT INTO AcceptedTable (Status, Name, Surname, Email, Password, DateOfEntry, PhoneNumber, QRCode, Image)
                                   SELECT Status, Name, Surname, Email, Password, DateOfEntry, PhoneNumber, QRCode, Image
                                   FROM RequestTable WHERE ID = @ID";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction);
                    insertCmd.Parameters.AddWithValue("@ID", id);
                    insertCmd.ExecuteNonQuery();

                    // Delete the user from RequestTable
                    string deleteQuery = "DELETE FROM RequestTable WHERE ID = @ID";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction);
                    deleteCmd.Parameters.AddWithValue("@ID", id);
                    deleteCmd.ExecuteNonQuery();

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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM RequestTable WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();
                LoadVisitors();
                MessageBox.Show("User Declined!");
            }
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            if (dgvVisitors.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["ID"].Value);
                DeclineUser(id);
            }

        }
    }
}

