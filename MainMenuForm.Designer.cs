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
            this.btnEmployee.Location = new System.Drawing.Point(338, 88);
            this.btnEmployee.Name = "btnEmployee";
            this.btnEmployee.Size = new System.Drawing.Size(162, 23);
            this.btnEmployee.TabIndex = 0;
            this.btnEmployee.Text = "Employee Acess";
            this.btnEmployee.UseVisualStyleBackColor = true;
            // 
            // btnVisitor
            // 
            this.btnVisitor.Location = new System.Drawing.Point(338, 165);
            this.btnVisitor.Name = "btnVisitor";
            this.btnVisitor.Size = new System.Drawing.Size(162, 23);
            this.btnVisitor.TabIndex = 1;
            this.btnVisitor.Text = "Visitor Acess";
            this.btnVisitor.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(338, 308);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(162, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // btnMange
            // 
            this.btnMange.Location = new System.Drawing.Point(338, 235);
            this.btnMange.Name = "btnMange";
            this.btnMange.Size = new System.Drawing.Size(162, 23);
            this.btnMange.TabIndex = 3;
            this.btnMange.Text = "Manage Users";
            this.btnMange.UseVisualStyleBackColor = true;
            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 544);
            this.Controls.Add(this.btnMange);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnVisitor);
            this.Controls.Add(this.btnEmployee);
            this.Name = "MainMenuForm";
            this.Text = "MainMenuForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEmployee;
        private System.Windows.Forms.Button btnVisitor;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnMange;
    }
}