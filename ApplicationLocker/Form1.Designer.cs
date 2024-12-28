namespace ApplicationLocker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtLog = new TextBox();
            btnAddApp = new Button();
            txtProcessName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtPassword = new TextBox();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            txtLog.Location = new Point(12, 12);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(416, 426);
            txtLog.TabIndex = 0;
            // 
            // btnAddApp
            // 
            btnAddApp.Location = new Point(434, 107);
            btnAddApp.Name = "btnAddApp";
            btnAddApp.Size = new Size(267, 28);
            btnAddApp.TabIndex = 1;
            btnAddApp.Text = "Add";
            btnAddApp.UseVisualStyleBackColor = true;
            btnAddApp.Click += btnAddApp_Click;
            // 
            // txtProcessName
            // 
            txtProcessName.Location = new Point(525, 40);
            txtProcessName.Name = "txtProcessName";
            txtProcessName.Size = new Size(176, 23);
            txtProcessName.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(434, 43);
            label1.Name = "label1";
            label1.Size = new Size(85, 15);
            label1.TabIndex = 3;
            label1.Text = "Process Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(434, 72);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 5;
            label2.Text = "Password:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(500, 69);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(201, 23);
            txtPassword.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(713, 450);
            Controls.Add(label2);
            Controls.Add(txtPassword);
            Controls.Add(label1);
            Controls.Add(txtProcessName);
            Controls.Add(btnAddApp);
            Controls.Add(txtLog);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtLog;
        private Button btnAddApp;
        private TextBox txtProcessName;
        private Label label1;
        private Label label2;
        private TextBox txtPassword;
    }
}
