namespace Ventrix.App.Popups
{
    partial class EditUserPopup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            guna2ShadowForm1 = new Guna.UI2.WinForms.Guna2ShadowForm(components);
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            lblTitle = new System.Windows.Forms.Label();
            txtSchoolId = new Guna.UI2.WinForms.Guna2TextBox();
            txtFirstName = new Guna.UI2.WinForms.Guna2TextBox();
            txtLastName = new Guna.UI2.WinForms.Guna2TextBox();
            txtSuffix = new Guna.UI2.WinForms.Guna2TextBox();
            cmbRole = new Guna.UI2.WinForms.Guna2ComboBox();
            btnSave = new Guna.UI2.WinForms.Guna2Button();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // guna2ShadowForm1
            // 
            guna2ShadowForm1.TargetForm = this;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 12;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle.Location = new System.Drawing.Point(25, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(176, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Edit User Account";
            // 
            // txtSchoolId
            // 
            txtSchoolId.BorderRadius = 8;
            txtSchoolId.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtSchoolId.DefaultText = "";
            txtSchoolId.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(252)))));
            txtSchoolId.DisabledState.ForeColor = System.Drawing.Color.Gray;
            txtSchoolId.Enabled = true; // Typically, School IDs shouldn't be changed, but you can enable this if needed
            txtSchoolId.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtSchoolId.Location = new System.Drawing.Point(30, 80);
            txtSchoolId.Name = "txtSchoolId";
            txtSchoolId.PlaceholderText = "School ID Number";
            txtSchoolId.Size = new System.Drawing.Size(340, 36);
            txtSchoolId.TabIndex = 1;
            // 
            // txtFirstName
            // 
            txtFirstName.BorderRadius = 8;
            txtFirstName.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtFirstName.DefaultText = "";
            txtFirstName.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtFirstName.Location = new System.Drawing.Point(30, 130);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.PlaceholderText = "First Name";
            txtFirstName.Size = new System.Drawing.Size(165, 36);
            txtFirstName.TabIndex = 2;
            // 
            // txtLastName
            // 
            txtLastName.BorderRadius = 8;
            txtLastName.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtLastName.DefaultText = "";
            txtLastName.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtLastName.Location = new System.Drawing.Point(205, 130);
            txtLastName.Name = "txtLastName";
            txtLastName.PlaceholderText = "Last Name";
            txtLastName.Size = new System.Drawing.Size(165, 36);
            txtLastName.TabIndex = 3;
            // 
            // txtSuffix
            // 
            txtSuffix.BorderRadius = 8;
            txtSuffix.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtSuffix.DefaultText = "";
            txtSuffix.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtSuffix.Location = new System.Drawing.Point(30, 180);
            txtSuffix.Name = "txtSuffix";
            txtSuffix.PlaceholderText = "Suffix (e.g., Jr, Sr)";
            txtSuffix.Size = new System.Drawing.Size(165, 36);
            txtSuffix.TabIndex = 4;
            // 
            // cmbRole
            // 
            cmbRole.BackColor = System.Drawing.Color.Transparent;
            cmbRole.BorderRadius = 8;
            cmbRole.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbRole.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            cmbRole.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            cmbRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            cmbRole.ItemHeight = 30;
            cmbRole.Location = new System.Drawing.Point(205, 180);
            cmbRole.Name = "cmbRole";
            cmbRole.Size = new System.Drawing.Size(165, 36);
            cmbRole.TabIndex = 5;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 8;
            btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            btnCancel.ForeColor = System.Drawing.Color.Black;
            btnCancel.Location = new System.Drawing.Point(135, 240);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(110, 40);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            btnCancel.Click += new System.EventHandler(btnCancel_Click);
            // 
            // btnSave
            // 
            btnSave.BorderRadius = 8;
            btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(71)))), ((int)(((byte)(161)))));
            btnSave.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Location = new System.Drawing.Point(260, 240);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(110, 40);
            btnSave.TabIndex = 7;
            btnSave.Text = "Save Changes";
            btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            btnSave.Click += new System.EventHandler(btnSave_Click);
            // 
            // EditUserPopup
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(400, 310);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(cmbRole);
            Controls.Add(txtSuffix);
            Controls.Add(txtLastName);
            Controls.Add(txtFirstName);
            Controls.Add(txtSchoolId);
            Controls.Add(lblTitle);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "EditUserPopup";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit User";
            Load += new System.EventHandler(EditUserPopup_Load);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2ShadowForm guna2ShadowForm1;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtSchoolId;
        private Guna.UI2.WinForms.Guna2TextBox txtFirstName;
        private Guna.UI2.WinForms.Guna2TextBox txtLastName;
        private Guna.UI2.WinForms.Guna2TextBox txtSuffix;
        private Guna.UI2.WinForms.Guna2ComboBox cmbRole;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
    }
}