using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Raza, H. (2021). BEST Way to SEND DATA between Multiple Forms | C# Windows Form. [online] www.youtube.com.
//Available at: https://youtu.be/t-4caAZKLJw [Accessed 11 Sep. 2024].

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
