namespace FacialRecognition
{
    partial class MainMenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEmployee = new System.Windows.Forms.Button();
            this.btnVisitor = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnMange = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEmployee
            // 
            this.btnEmployee.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnEmployee.Location = new System.Drawing.Point(303, 84);
            this.btnEmployee.Name = "btnEmployee";
            this.btnEmployee.Size = new System.Drawing.Size(162, 39);
            this.btnEmployee.TabIndex = 0;
            this.btnEmployee.Text = "Employee Acess";
            this.btnEmployee.UseVisualStyleBackColor = false;
            this.btnEmployee.Click += new System.EventHandler(this.btnEmployee_Click);
            // 
            // btnVisitor
            // 
            this.btnVisitor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVisitor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnVisitor.Location = new System.Drawing.Point(306, 165);
            this.btnVisitor.Name = "btnVisitor";
            this.btnVisitor.Size = new System.Drawing.Size(162, 38);
            this.btnVisitor.TabIndex = 1;
            this.btnVisitor.Text = "Visitor Acess";
            this.btnVisitor.UseVisualStyleBackColor = false;
            this.btnVisitor.Click += new System.EventHandler(this.btnVisitor_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnExit.Location = new System.Drawing.Point(306, 328);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(162, 38);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnMange
            // 
            this.btnMange.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnMange.Location = new System.Drawing.Point(305, 246);
            this.btnMange.Name = "btnMange";
            this.btnMange.Size = new System.Drawing.Size(162, 39);
            this.btnMange.TabIndex = 3;
            this.btnMange.Text = "Manage Users";
            this.btnMange.UseVisualStyleBackColor = false;
            this.btnMange.Click += new System.EventHandler(this.btnMange_Click);
            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 527);
            this.Controls.Add(this.btnMange);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnVisitor);
            this.Controls.Add(this.btnEmployee);
            this.Name = "MainMenuForm";
            this.Text = "Main Menu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEmployee;
        private System.Windows.Forms.Button btnVisitor;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnMange;
    }
}