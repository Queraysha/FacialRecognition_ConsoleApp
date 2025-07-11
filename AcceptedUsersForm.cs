using ManageStoredUsersForm.cs;
using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace FacialRecognition
{
    public partial class AcceptedUsersForm : Form
    {
        private readonly string connectionString;
        private ContextMenuStrip gridContextMenu;

        public AcceptedUsersForm()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();

            connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_PASS")};" +
                               $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                               $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERT")};";

            SetupContextMenu();
            LoadAcceptedUsers();
        }

        private void SetupContextMenu()
        {
            gridContextMenu = new ContextMenuStrip();
            gridContextMenu.Items.Add("Update", null, (s, e) => UpdateSelectedRow());
            
            dgvEmployee.ContextMenuStrip = gridContextMenu;
            dgvVisitors.ContextMenuStrip = gridContextMenu;

            // Add double-click event handlers
            dgvEmployee.CellDoubleClick += (s, e) => UpdateSelectedRow();
            dgvVisitors.CellDoubleClick += (s, e) => UpdateSelectedRow();
        }

        private void UpdateSelectedRow()
        {
            if (dgvEmployee.SelectedRows.Count > 0)
            {
                var row = dgvEmployee.SelectedRows[0];

                int id = Convert.ToInt32(row.Cells["emp_id"].Value);
                string rid = row.Cells["rid"].Value?.ToString() ?? "";
                string name = row.Cells["first_name"].Value?.ToString() ?? "";
                string surname = row.Cells["last_name"].Value?.ToString() ?? "";
                string email = row.Cells["email"].Value?.ToString() ?? "";
                string password = row.Cells["password"].Value?.ToString() ?? "";
                string phoneNumber = row.Cells["phone"]?.Value?.ToString() ?? "";
                string qrCode = row.Cells["qr_code_path"]?.Value?.ToString() ?? "";
                string imagePath = row.Cells["facial_path"]?.Value?.ToString() ?? "";

                var updateForm = new UpdateUserForm("employee", id, rid, name, surname, email, password, phoneNumber, qrCode, imagePath);
                if (updateForm.ShowDialog() == DialogResult.OK)
                {
                    LoadAcceptedUsers(); // Refresh data if update was successful
                }
            }
            else if (dgvVisitors.SelectedRows.Count > 0)
            {
                var row = dgvVisitors.SelectedRows[0];

                int id = Convert.ToInt32(row.Cells["visitor_id"].Value);
                string rid = row.Cells["rid"].Value?.ToString() ?? "";
                string name = row.Cells["first_name"].Value?.ToString() ?? "";
                string surname = row.Cells["last_name"].Value?.ToString() ?? "";
                string email = row.Cells["email"].Value?.ToString() ?? "";
                string phoneNumber = row.Cells["phone"]?.Value?.ToString() ?? "";
                string qrCode = row.Cells["qr_code_path"]?.Value?.ToString() ?? "";

                var updateForm = new UpdateUserForm("visitor", id, rid, name, surname, email, "", phoneNumber, qrCode, "");
                if (updateForm.ShowDialog() == DialogResult.OK)
                {
                    LoadAcceptedUsers(); // Refresh data if update was successful
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadAcceptedUsers()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Load employees
                var employeeAdapter = new NpgsqlDataAdapter("SELECT * FROM employees", conn);
                var employeeTable = new DataTable();
                employeeAdapter.Fill(employeeTable);
                dgvEmployee.DataSource = employeeTable;

                // Load visitors
                var visitorAdapter = new NpgsqlDataAdapter("SELECT * FROM visitors", conn);
                var visitorTable = new DataTable();
                visitorAdapter.Fill(visitorTable);
                dgvVisitors.DataSource = visitorTable;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateSelectedRow();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvEmployee.CurrentRow.Cells["emp_id"].Value);
                RemoveUser(id, "employees");
            }
            else if (dgvVisitors.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvVisitors.CurrentRow.Cells["visitor_id"].Value);
                RemoveUser(id, "visitors");
            }
            else
            {
                MessageBox.Show("Please select a user to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoveUser(int id, string tableName)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string idColumn = tableName == "employees" ? "emp_id" : "visitor_id";
                string query = $"DELETE FROM {tableName} WHERE {idColumn} = @ID";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User removed successfully!");
                    LoadAcceptedUsers();
                }
            }
        }

        private void AcceptedUsersForm_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
    
            MainMenuForm mm = new MainMenuForm();
            mm.Show();
            this.Hide(); // Hide Admin Form
        }
    }
}
