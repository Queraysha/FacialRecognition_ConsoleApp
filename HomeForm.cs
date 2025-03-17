using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacialRecognition
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Open the Report Issues form (Raza,2021)
            AdminLoginForm LoginForm = new AdminLoginForm();
            LoginForm.Show();
            this.Hide(); // Hide Main Menu Form
        }
    }
}
