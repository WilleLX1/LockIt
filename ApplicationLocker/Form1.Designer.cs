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
            checkedListBoxTargets = new CheckedListBox();
            btnRemoveApp = new Button();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Location = new Point(12, 12);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(376, 426);
            txtLog.TabIndex = 0;
            // 
            // btnAddApp
            // 
            btnAddApp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnAddApp.Location = new Point(6, 112);
            btnAddApp.Name = "btnAddApp";
            btnAddApp.Size = new Size(295, 28);
            btnAddApp.TabIndex = 1;
            btnAddApp.Text = "Add";
            btnAddApp.UseVisualStyleBackColor = true;
            btnAddApp.Click += btnAddApp_Click;
            // 
            // txtProcessName
            // 
            txtProcessName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtProcessName.Location = new Point(97, 28);
            txtProcessName.Name = "txtProcessName";
            txtProcessName.Size = new Size(204, 23);
            txtProcessName.TabIndex = 2;
            txtProcessName.Text = "notepad";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 31);
            label1.Name = "label1";
            label1.Size = new Size(85, 15);
            label1.TabIndex = 3;
            label1.Text = "Process Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 60);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 5;
            label2.Text = "Password:";
            // 
            // txtPassword
            // 
            txtPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPassword.Location = new Point(72, 57);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(229, 23);
            txtPassword.TabIndex = 4;
            txtPassword.Text = "1234";
            // 
            // checkedListBoxTargets
            // 
            checkedListBoxTargets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            checkedListBoxTargets.FormattingEnabled = true;
            checkedListBoxTargets.Location = new Point(394, 164);
            checkedListBoxTargets.Name = "checkedListBoxTargets";
            checkedListBoxTargets.Size = new Size(307, 238);
            checkedListBoxTargets.TabIndex = 6;
            checkedListBoxTargets.ItemCheck += checkedListBoxTargets_ItemCheck;
            checkedListBoxTargets.SelectedIndexChanged += checkedListBoxTargets_SelectedIndexChanged;
            // 
            // btnRemoveApp
            // 
            btnRemoveApp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRemoveApp.Location = new Point(394, 408);
            btnRemoveApp.Name = "btnRemoveApp";
            btnRemoveApp.Size = new Size(307, 30);
            btnRemoveApp.TabIndex = 7;
            btnRemoveApp.Text = "Remove Selected";
            btnRemoveApp.UseVisualStyleBackColor = true;
            btnRemoveApp.Click += btnRemoveApp_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox1.Controls.Add(btnAddApp);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtProcessName);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtPassword);
            groupBox1.Location = new Point(394, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(307, 146);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Add Processes:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(713, 450);
            Controls.Add(groupBox1);
            Controls.Add(btnRemoveApp);
            Controls.Add(checkedListBoxTargets);
            Controls.Add(txtLog);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
        public CheckedListBox checkedListBoxTargets;
        private Button btnRemoveApp;
        private GroupBox groupBox1;
    }
}
