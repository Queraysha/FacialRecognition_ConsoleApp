using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacialRecognition
{
    public partial class MainMenuForm: Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            // Open the Main Menu Form form (Raza,2021)
            EmployeeForm employee = new EmployeeForm();
            employee.Show();
            this.Hide(); // Hide Admin Form
        }

        private void btnMange_Click(object sender, EventArgs e)
        {
            // Open the Main Menu Form form (Raza,2021)
            AcceptedUsersForm au = new AcceptedUsersForm();
            au.Show();
            this.Hide(); // Hide Admin Form
        }

        private void btnVisitor_Click(object sender, EventArgs e)
        {

        }
    }
}
