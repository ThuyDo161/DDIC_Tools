namespace DDIC_Tools
{
    partial class FormImportSchedule
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
            this.btnImport = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.grbSelect3 = new System.Windows.Forms.GroupBox();
            this.cbCat3 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grbSelect2 = new System.Windows.Forms.GroupBox();
            this.cbCat2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbSelect1 = new System.Windows.Forms.GroupBox();
            this.cbCat1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.grbSelect3.SuspendLayout();
            this.grbSelect2.SuspendLayout();
            this.grbSelect1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnImport.FlatAppearance.BorderSize = 0;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Location = new System.Drawing.Point(284, 355);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(163, 40);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtPath);
            this.panel1.Controls.Add(this.btnSelectFile);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.grbSelect3);
            this.panel1.Controls.Add(this.grbSelect2);
            this.panel1.Controls.Add(this.grbSelect1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(741, 320);
            this.panel1.TabIndex = 2;
            // 
            // txtPath
            // 
            this.txtPath.Enabled = false;
            this.txtPath.Location = new System.Drawing.Point(191, 276);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(529, 20);
            this.txtPath.TabIndex = 5;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(110, 276);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(75, 20);
            this.btnSelectFile.TabIndex = 4;
            this.btnSelectFile.Text = "Browser";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 280);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Chọn file import";
            // 
            // grbSelect3
            // 
            this.grbSelect3.Controls.Add(this.cbCat3);
            this.grbSelect3.Controls.Add(this.label3);
            this.grbSelect3.Location = new System.Drawing.Point(20, 180);
            this.grbSelect3.Name = "grbSelect3";
            this.grbSelect3.Size = new System.Drawing.Size(700, 70);
            this.grbSelect3.TabIndex = 2;
            this.grbSelect3.TabStop = false;
            // 
            // cbCat3
            // 
            this.cbCat3.FormattingEnabled = true;
            this.cbCat3.Location = new System.Drawing.Point(5, 35);
            this.cbCat3.Name = "cbCat3";
            this.cbCat3.Size = new System.Drawing.Size(280, 21);
            this.cbCat3.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Choose parameter 3";
            // 
            // grbSelect2
            // 
            this.grbSelect2.Controls.Add(this.cbCat2);
            this.grbSelect2.Controls.Add(this.label2);
            this.grbSelect2.Location = new System.Drawing.Point(20, 90);
            this.grbSelect2.Name = "grbSelect2";
            this.grbSelect2.Size = new System.Drawing.Size(700, 70);
            this.grbSelect2.TabIndex = 1;
            this.grbSelect2.TabStop = false;
            // 
            // cbCat2
            // 
            this.cbCat2.FormattingEnabled = true;
            this.cbCat2.Location = new System.Drawing.Point(5, 35);
            this.cbCat2.Name = "cbCat2";
            this.cbCat2.Size = new System.Drawing.Size(280, 21);
            this.cbCat2.TabIndex = 1;
            this.cbCat2.SelectedIndexChanged += new System.EventHandler(this.cbCat2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Choose parameter 2";
            // 
            // grbSelect1
            // 
            this.grbSelect1.Controls.Add(this.cbCat1);
            this.grbSelect1.Controls.Add(this.label1);
            this.grbSelect1.Location = new System.Drawing.Point(20, 10);
            this.grbSelect1.Name = "grbSelect1";
            this.grbSelect1.Size = new System.Drawing.Size(700, 70);
            this.grbSelect1.TabIndex = 0;
            this.grbSelect1.TabStop = false;
            // 
            // cbCat1
            // 
            this.cbCat1.FormattingEnabled = true;
            this.cbCat1.Location = new System.Drawing.Point(5, 35);
            this.cbCat1.Name = "cbCat1";
            this.cbCat1.Size = new System.Drawing.Size(280, 21);
            this.cbCat1.TabIndex = 1;
            this.cbCat1.SelectedIndexChanged += new System.EventHandler(this.cbCat1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose parameter 1";
            // 
            // FormImportSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 414);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.panel1);
            this.Name = "FormImportSchedule";
            this.Text = "Create schedule from file txt";
            this.Load += new System.EventHandler(this.FormImportSchedule_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grbSelect3.ResumeLayout(false);
            this.grbSelect3.PerformLayout();
            this.grbSelect2.ResumeLayout(false);
            this.grbSelect2.PerformLayout();
            this.grbSelect1.ResumeLayout(false);
            this.grbSelect1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grbSelect3;
        private System.Windows.Forms.ComboBox cbCat3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grbSelect2;
        private System.Windows.Forms.ComboBox cbCat2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grbSelect1;
        private System.Windows.Forms.ComboBox cbCat1;
        private System.Windows.Forms.Label label1;
    }
}