namespace DDIC_Tools.FormUI
{
    partial class FormEditTitleSchedule
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.flowLayoutPanel = new System.Windows.Forms.Panel();
            this.grbSelect = new System.Windows.Forms.GroupBox();
            this.txtColumn = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel.SuspendLayout();
            this.grbSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(186, 460);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(314, 42);
            this.btnEdit.TabIndex = 17;
            this.btnEdit.Text = "Chỉnh sửa tiêu đề";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.Controls.Add(this.grbSelect);
            this.flowLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(741, 424);
            this.flowLayoutPanel.TabIndex = 16;
            // 
            // grbSelect
            // 
            this.grbSelect.Controls.Add(this.txtColumn);
            this.grbSelect.Controls.Add(this.btnAdd);
            this.grbSelect.Controls.Add(this.label3);
            this.grbSelect.Location = new System.Drawing.Point(20, 10);
            this.grbSelect.Name = "grbSelect";
            this.grbSelect.Size = new System.Drawing.Size(700, 95);
            this.grbSelect.TabIndex = 17;
            this.grbSelect.TabStop = false;
            // 
            // txtColumn
            // 
            this.txtColumn.Location = new System.Drawing.Point(5, 36);
            this.txtColumn.Multiline = true;
            this.txtColumn.Name = "txtColumn";
            this.txtColumn.Size = new System.Drawing.Size(590, 40);
            this.txtColumn.TabIndex = 13;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.btnAdd.Location = new System.Drawing.Point(610, 30);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(70, 31);
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "Thêm";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Nhập tên cột 1";
            // 
            // FormEditTitleSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 527);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "FormEditTitleSchedule";
            this.Text = "Chỉnh sửa tiêu đề các cột trong bảng thống kê";
            this.Load += new System.EventHandler(this.FormEditTitleSchedule_Load);
            this.flowLayoutPanel.ResumeLayout(false);
            this.grbSelect.ResumeLayout(false);
            this.grbSelect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Panel flowLayoutPanel;
        private System.Windows.Forms.GroupBox grbSelect;
        private System.Windows.Forms.TextBox txtColumn;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label3;
    }
}