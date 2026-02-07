namespace Ventrix.UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panelLoginCard = new Panel();
            pictureBox3 = new PictureBox();
            panelUserWrapper = new Panel();
            textBoxName = new TextBox();
            panelUnderline1 = new Panel();
            panelPassWrapper = new Panel();
            panelUnderline2 = new Panel();
            btnToggle = new Button();
            textBoxPassword = new TextBox();
            btnSignIn = new Button();
            pictureBoxLogo = new PictureBox();
            lblName = new Label();
            lblPassword = new Label();
            labelCreateAccount = new Label();
            linkLabelCreateAccount = new LinkLabel();
            checkBoxRemember = new CheckBox();
            linkLabelForgotPass = new LinkLabel();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            btnMinimized = new Button();
            btnMaximized = new Button();
            btnClose = new Button();
            panelGlowWrapper = new Panel();
            panelLoginCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            panelUserWrapper.SuspendLayout();
            panelPassWrapper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panelGlowWrapper.SuspendLayout();
            SuspendLayout();
            // 
            // panelLoginCard
            // 
            panelLoginCard.Anchor = AnchorStyles.None;
            panelLoginCard.BackColor = Color.White;
            panelLoginCard.BackgroundImageLayout = ImageLayout.Center;
            panelLoginCard.Controls.Add(pictureBox3);
            panelLoginCard.Controls.Add(panelUserWrapper);
            panelLoginCard.Controls.Add(panelPassWrapper);
            panelLoginCard.Controls.Add(btnSignIn);
            panelLoginCard.Controls.Add(pictureBoxLogo);
            panelLoginCard.Controls.Add(lblName);
            panelLoginCard.Controls.Add(lblPassword);
            panelLoginCard.Controls.Add(labelCreateAccount);
            panelLoginCard.Controls.Add(linkLabelCreateAccount);
            panelLoginCard.Controls.Add(checkBoxRemember);
            panelLoginCard.Controls.Add(linkLabelForgotPass);
            panelLoginCard.Font = new Font("Sitka Banner", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            panelLoginCard.ForeColor = SystemColors.ControlText;
            panelLoginCard.Location = new Point(1, 1);
            panelLoginCard.Margin = new Padding(0);
            panelLoginCard.Name = "panelLoginCard";
            panelLoginCard.Padding = new Padding(20);
            panelLoginCard.Size = new Size(592, 692);
            panelLoginCard.TabIndex = 1;
          
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(183, 23);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(314, 89);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // panelUserWrapper
            // 
            panelUserWrapper.BackColor = Color.LightGray;
            panelUserWrapper.BackgroundImageLayout = ImageLayout.Stretch;
            panelUserWrapper.Controls.Add(textBoxName);
            panelUserWrapper.Controls.Add(panelUnderline1);
            panelUserWrapper.Location = new Point(53, 230);
            panelUserWrapper.Name = "panelUserWrapper";
            panelUserWrapper.Padding = new Padding(1);
            panelUserWrapper.Size = new Size(499, 37);
            panelUserWrapper.TabIndex = 13;
            // 
            // textBoxName
            // 
            textBoxName.BorderStyle = BorderStyle.None;
            textBoxName.Location = new Point(1, 1);
            textBoxName.Multiline = true;
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(497, 33);
            textBoxName.TabIndex = 0;
            textBoxName.Enter += textBoxName_Enter;
            textBoxName.Leave += textBoxName_Leave;
            // 
            // panelUnderline1
            // 
            panelUnderline1.BackColor = Color.LightGray;
            panelUnderline1.Dock = DockStyle.Bottom;
            panelUnderline1.Location = new Point(1, 34);
            panelUnderline1.Name = "panelUnderline1";
            panelUnderline1.Size = new Size(497, 2);
            panelUnderline1.TabIndex = 11;
            panelUnderline1.Enter += textBoxName_Enter;
            panelUnderline1.Leave += textBoxName_Leave;
            // 
            // panelPassWrapper
            // 
            panelPassWrapper.BackColor = Color.LightGray;
            panelPassWrapper.BackgroundImageLayout = ImageLayout.Stretch;
            panelPassWrapper.Controls.Add(panelUnderline2);
            panelPassWrapper.Controls.Add(btnToggle);
            panelPassWrapper.Controls.Add(textBoxPassword);
            panelPassWrapper.Location = new Point(56, 365);
            panelPassWrapper.Name = "panelPassWrapper";
            panelPassWrapper.Padding = new Padding(1);
            panelPassWrapper.Size = new Size(495, 39);
            panelPassWrapper.TabIndex = 14;
            // 
            // panelUnderline2
            // 
            panelUnderline2.BackColor = Color.LightGray;
            panelUnderline2.Dock = DockStyle.Bottom;
            panelUnderline2.Location = new Point(1, 36);
            panelUnderline2.Name = "panelUnderline2";
            panelUnderline2.Size = new Size(493, 2);
            panelUnderline2.TabIndex = 12;
            panelUnderline2.Enter += textBoxPassword_Enter;
            panelUnderline2.Leave += textBoxPassword_Leave;
            // 
            // btnToggle
            // 
            btnToggle.BackColor = Color.White;
            btnToggle.BackgroundImage = Properties.Resources.eye;
            btnToggle.BackgroundImageLayout = ImageLayout.Zoom;
            btnToggle.Location = new Point(464, -1);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new Size(32, 39);
            btnToggle.TabIndex = 0;
            btnToggle.UseVisualStyleBackColor = false;
            btnToggle.Click += btnToggle_Click;
            // 
            // textBoxPassword
            // 
            textBoxPassword.BorderStyle = BorderStyle.None;
            textBoxPassword.Location = new Point(1, 1);
            textBoxPassword.Multiline = true;
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(493, 37);
            textBoxPassword.TabIndex = 5;
            textBoxPassword.Enter += textBoxPassword_Enter;
            textBoxPassword.Leave += textBoxPassword_Leave;
            // 
            // btnSignIn
            // 
            btnSignIn.BackColor = Color.DarkBlue;
            btnSignIn.FlatStyle = FlatStyle.Flat;
            btnSignIn.ForeColor = Color.White;
            btnSignIn.Location = new Point(53, 512);
            btnSignIn.Name = "btnSignIn";
            btnSignIn.Size = new Size(499, 41);
            btnSignIn.TabIndex = 6;
            btnSignIn.Text = "Log in";
            btnSignIn.UseVisualStyleBackColor = false;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.BackColor = Color.Transparent;
            pictureBoxLogo.BackgroundImageLayout = ImageLayout.None;
            pictureBoxLogo.Image = Properties.Resources.Logo;
            pictureBoxLogo.Location = new Point(103, 13);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(95, 99);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxLogo.TabIndex = 1;
            pictureBoxLogo.TabStop = false;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.BackColor = Color.Transparent;
            lblName.Location = new Point(49, 198);
            lblName.Name = "lblName";
            lblName.Size = new Size(160, 29);
            lblName.TabIndex = 3;
            lblName.Text = "Username or Email";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.BackColor = Color.Transparent;
            lblPassword.Location = new Point(53, 333);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(86, 29);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Password";
            // 
            // labelCreateAccount
            // 
            labelCreateAccount.AutoSize = true;
            labelCreateAccount.BackColor = Color.Transparent;
            labelCreateAccount.Location = new Point(152, 556);
            labelCreateAccount.Name = "labelCreateAccount";
            labelCreateAccount.Size = new Size(235, 29);
            labelCreateAccount.TabIndex = 10;
            labelCreateAccount.Text = "Doesn't have an account yet?";
            // 
            // linkLabelCreateAccount
            // 
            linkLabelCreateAccount.AutoSize = true;
            linkLabelCreateAccount.BackColor = Color.Transparent;
            linkLabelCreateAccount.Location = new Point(393, 556);
            linkLabelCreateAccount.Name = "linkLabelCreateAccount";
            linkLabelCreateAccount.RightToLeft = RightToLeft.No;
            linkLabelCreateAccount.Size = new Size(72, 29);
            linkLabelCreateAccount.TabIndex = 9;
            linkLabelCreateAccount.TabStop = true;
            linkLabelCreateAccount.Text = "Sign up";
            // 
            // checkBoxRemember
            // 
            checkBoxRemember.AutoSize = true;
            checkBoxRemember.BackColor = Color.Transparent;
            checkBoxRemember.Location = new Point(56, 476);
            checkBoxRemember.Name = "checkBoxRemember";
            checkBoxRemember.Size = new Size(144, 33);
            checkBoxRemember.TabIndex = 8;
            checkBoxRemember.Text = "Remember me";
            checkBoxRemember.UseVisualStyleBackColor = false;
            // 
            // linkLabelForgotPass
            // 
            linkLabelForgotPass.AutoSize = true;
            linkLabelForgotPass.BackColor = Color.Transparent;
            linkLabelForgotPass.Location = new Point(401, 476);
            linkLabelForgotPass.Name = "linkLabelForgotPass";
            linkLabelForgotPass.Size = new Size(149, 29);
            linkLabelForgotPass.TabIndex = 7;
            linkLabelForgotPass.TabStop = true;
            linkLabelForgotPass.Text = "Forgot Password?";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.BackgroundImageLayout = ImageLayout.Stretch;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 600F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel1, 2, 0);
            tableLayoutPanel1.Controls.Add(panelGlowWrapper, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 700F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1280, 800);
            tableLayoutPanel1.TabIndex = 2;
            tableLayoutPanel1.MouseDown += tableLayoutPanel1_MouseDown;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.BackColor = Color.Transparent;
            panel1.BackgroundImageLayout = ImageLayout.Zoom;
            panel1.Controls.Add(btnMinimized);
            panel1.Controls.Add(btnMaximized);
            panel1.Controls.Add(btnClose);
            panel1.Location = new Point(1027, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 38);
            panel1.TabIndex = 15;
            // 
            // btnMinimized
            // 
            btnMinimized.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimized.BackgroundImage = (Image)resources.GetObject("btnMinimized.BackgroundImage");
            btnMinimized.BackgroundImageLayout = ImageLayout.Zoom;
            btnMinimized.FlatAppearance.BorderSize = 0;
            btnMinimized.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 255, 255, 255);
            btnMinimized.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 255, 255, 255);
            btnMinimized.FlatStyle = FlatStyle.Flat;
            btnMinimized.Location = new Point(111, 3);
            btnMinimized.Name = "btnMinimized";
            btnMinimized.Size = new Size(25, 35);
            btnMinimized.TabIndex = 4;
            btnMinimized.UseVisualStyleBackColor = true;
            btnMinimized.Click += btnMinimized_Click;
            // 
            // btnMaximized
            // 
            btnMaximized.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMaximized.BackgroundImage = (Image)resources.GetObject("btnMaximized.BackgroundImage");
            btnMaximized.BackgroundImageLayout = ImageLayout.Zoom;
            btnMaximized.FlatAppearance.BorderSize = 0;
            btnMaximized.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 255, 255, 255);
            btnMaximized.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 255, 255, 255);
            btnMaximized.FlatStyle = FlatStyle.Flat;
            btnMaximized.ForeColor = SystemColors.Desktop;
            btnMaximized.Location = new Point(164, 5);
            btnMaximized.Name = "btnMaximized";
            btnMaximized.Size = new Size(37, 32);
            btnMaximized.TabIndex = 3;
            btnMaximized.UseVisualStyleBackColor = true;
            btnMaximized.Click += btnMaximized_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackgroundImage = (Image)resources.GetObject("btnClose.BackgroundImage");
            btnClose.BackgroundImageLayout = ImageLayout.Zoom;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseDownBackColor = Color.Firebrick;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 255, 255, 255);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(218, 5);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(23, 28);
            btnClose.TabIndex = 2;
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // panelGlowWrapper
            // 
            panelGlowWrapper.AutoSize = true;
            panelGlowWrapper.BackColor = Color.DimGray;
            panelGlowWrapper.Controls.Add(panelLoginCard);
            panelGlowWrapper.Dock = DockStyle.Fill;
            panelGlowWrapper.Location = new Point(343, 53);
            panelGlowWrapper.Name = "panelGlowWrapper";
            panelGlowWrapper.Padding = new Padding(1);
            panelGlowWrapper.Size = new Size(594, 694);
            panelGlowWrapper.TabIndex = 16;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1280, 800);
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            Resize += Form1_Resize;
            panelLoginCard.ResumeLayout(false);
            panelLoginCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            panelUserWrapper.ResumeLayout(false);
            panelUserWrapper.PerformLayout();
            panelPassWrapper.ResumeLayout(false);
            panelPassWrapper.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panelGlowWrapper.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Panel panelLoginCard;
        private PictureBox pictureBox3;
        private PictureBox pictureBoxLogo;
        private TextBox textBoxName;
        private Label lblPassword;
        private Label lblName;
        private Button btnSignIn;
        private TextBox textBoxPassword;
        private LinkLabel linkLabelCreateAccount;
        private CheckBox checkBoxRemember;
        private LinkLabel linkLabelForgotPass;
        private Label labelCreateAccount;
        private Panel panelUnderline1;
        private Panel panelUnderline2;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnToggle;
        private Panel panelUserWrapper;
        private Panel panelPassWrapper;
        private Button btnClose;
        private Panel panel1;
        private Button btnMinimized;
        private Button btnMaximized;
        private Panel panelGlowWrapper;
    }
}
