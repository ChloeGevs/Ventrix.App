namespace Ventrix.App
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            btnMinimize = new Button();
            btnMaximize = new Button();
            btnClose = new Button();
            panelGlowWrapper = new Panel();
            panelCreateAccCard = new Panel();
            btnToggleConfirmPass = new Button();
            btnTogglePassword = new Button();
            textBoxPassword = new TextBox();
            linkLabelGoToLogin = new LinkLabel();
            labelGoToLogin = new Label();
            btnSignup = new Button();
            labelConfirmPass = new Label();
            textBoxConfirmPass = new TextBox();
            labelBusinessName = new Label();
            textBoxBusinessName = new TextBox();
            labelPassword = new Label();
            labelEmail = new Label();
            labelCreateAcc = new Label();
            textBoxEmail = new TextBox();
            panel2 = new Panel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panelGlowWrapper.SuspendLayout();
            panelCreateAccCard.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 550F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel1, 2, 0);
            tableLayoutPanel1.Controls.Add(panelGlowWrapper, 1, 1);
            tableLayoutPanel1.Controls.Add(panel2, 1, 0);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 650F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1280, 800);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.BackgroundImageLayout = ImageLayout.Zoom;
            panel1.Controls.Add(btnMinimize);
            panel1.Controls.Add(btnMaximize);
            panel1.Controls.Add(btnClose);
            panel1.Location = new Point(918, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(359, 69);
            panel1.TabIndex = 0;
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = AnchorStyles.None;
            btnMinimize.BackColor = Color.Transparent;
            btnMinimize.BackgroundImage = Properties.Resources.minimize;
            btnMinimize.BackgroundImageLayout = ImageLayout.Zoom;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 255, 255, 255);
            btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 255, 255, 255);
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.Location = new Point(214, 3);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(30, 35);
            btnMinimize.TabIndex = 2;
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += btnMinimize_Click;
            // 
            // btnMaximize
            // 
            btnMaximize.Anchor = AnchorStyles.None;
            btnMaximize.BackgroundImage = Properties.Resources.maximize;
            btnMaximize.BackgroundImageLayout = ImageLayout.Zoom;
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 255, 255, 255);
            btnMaximize.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 255, 255, 255);
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.Location = new Point(267, 3);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new Size(30, 35);
            btnMaximize.TabIndex = 1;
            btnMaximize.UseVisualStyleBackColor = true;
            btnMaximize.Click += btnMaximize_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.None;
            btnClose.BackColor = Color.Transparent;
            btnClose.BackgroundImage = Properties.Resources.close;
            btnClose.BackgroundImageLayout = ImageLayout.Zoom;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseDownBackColor = Color.Firebrick;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 255, 255, 255);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(320, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(30, 35);
            btnClose.TabIndex = 0;
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // panelGlowWrapper
            // 
            panelGlowWrapper.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelGlowWrapper.BackColor = Color.DimGray;
            panelGlowWrapper.Controls.Add(panelCreateAccCard);
            panelGlowWrapper.Location = new Point(368, 78);
            panelGlowWrapper.Name = "panelGlowWrapper";
            panelGlowWrapper.Padding = new Padding(1);
            panelGlowWrapper.Size = new Size(544, 644);
            panelGlowWrapper.TabIndex = 1;
            // 
            // panelCreateAccCard
            // 
            panelCreateAccCard.BackColor = Color.White;
            panelCreateAccCard.BackgroundImageLayout = ImageLayout.Zoom;
            panelCreateAccCard.Controls.Add(btnToggleConfirmPass);
            panelCreateAccCard.Controls.Add(btnTogglePassword);
            panelCreateAccCard.Controls.Add(textBoxPassword);
            panelCreateAccCard.Controls.Add(linkLabelGoToLogin);
            panelCreateAccCard.Controls.Add(labelGoToLogin);
            panelCreateAccCard.Controls.Add(btnSignup);
            panelCreateAccCard.Controls.Add(labelConfirmPass);
            panelCreateAccCard.Controls.Add(textBoxConfirmPass);
            panelCreateAccCard.Controls.Add(labelBusinessName);
            panelCreateAccCard.Controls.Add(textBoxBusinessName);
            panelCreateAccCard.Controls.Add(labelPassword);
            panelCreateAccCard.Controls.Add(labelEmail);
            panelCreateAccCard.Controls.Add(labelCreateAcc);
            panelCreateAccCard.Controls.Add(textBoxEmail);
            panelCreateAccCard.Dock = DockStyle.Fill;
            panelCreateAccCard.Font = new Font("Sitka Banner", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            panelCreateAccCard.Location = new Point(1, 1);
            panelCreateAccCard.Margin = new Padding(0);
            panelCreateAccCard.Name = "panelCreateAccCard";
            panelCreateAccCard.Padding = new Padding(20);
            panelCreateAccCard.RightToLeft = RightToLeft.No;
            panelCreateAccCard.Size = new Size(542, 642);
            panelCreateAccCard.TabIndex = 0;
            // 
            // btnToggleConfirmPass
            // 
            btnToggleConfirmPass.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnToggleConfirmPass.BackColor = Color.Transparent;
            btnToggleConfirmPass.BackgroundImage = Properties.Resources.eye;
            btnToggleConfirmPass.BackgroundImageLayout = ImageLayout.Zoom;
            btnToggleConfirmPass.Location = new Point(445, 294);
            btnToggleConfirmPass.Name = "btnToggleConfirmPass";
            btnToggleConfirmPass.Size = new Size(40, 30);
            btnToggleConfirmPass.TabIndex = 13;
            btnToggleConfirmPass.UseVisualStyleBackColor = false;
            // 
            // btnTogglePassword
            // 
            btnTogglePassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnTogglePassword.BackColor = Color.Transparent;
            btnTogglePassword.BackgroundImage = Properties.Resources.eye;
            btnTogglePassword.BackgroundImageLayout = ImageLayout.Zoom;
            btnTogglePassword.Location = new Point(445, 204);
            btnTogglePassword.Name = "btnTogglePassword";
            btnTogglePassword.Size = new Size(41, 30);
            btnTogglePassword.TabIndex = 12;
            btnTogglePassword.UseVisualStyleBackColor = false;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPassword.Location = new Point(49, 204);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(436, 33);
            textBoxPassword.TabIndex = 3;
            // 
            // linkLabelGoToLogin
            // 
            linkLabelGoToLogin.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            linkLabelGoToLogin.AutoSize = true;
            linkLabelGoToLogin.Location = new Point(328, 560);
            linkLabelGoToLogin.Name = "linkLabelGoToLogin";
            linkLabelGoToLogin.Size = new Size(61, 29);
            linkLabelGoToLogin.TabIndex = 11;
            linkLabelGoToLogin.TabStop = true;
            linkLabelGoToLogin.Text = "Log in";
            // 
            // labelGoToLogin
            // 
            labelGoToLogin.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelGoToLogin.AutoSize = true;
            labelGoToLogin.Location = new Point(135, 560);
            labelGoToLogin.Name = "labelGoToLogin";
            labelGoToLogin.Size = new Size(211, 29);
            labelGoToLogin.TabIndex = 10;
            labelGoToLogin.Text = "Already have an account?";
            // 
            // btnSignup
            // 
            btnSignup.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSignup.BackColor = Color.DarkBlue;
            btnSignup.BackgroundImageLayout = ImageLayout.None;
            btnSignup.FlatStyle = FlatStyle.Flat;
            btnSignup.ForeColor = Color.White;
            btnSignup.Location = new Point(49, 500);
            btnSignup.Name = "btnSignup";
            btnSignup.Size = new Size(435, 57);
            btnSignup.TabIndex = 9;
            btnSignup.Text = "CREATE ACCOUNT";
            btnSignup.UseVisualStyleBackColor = false;
            // 
            // labelConfirmPass
            // 
            labelConfirmPass.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelConfirmPass.AutoSize = true;
            labelConfirmPass.Location = new Point(49, 265);
            labelConfirmPass.Name = "labelConfirmPass";
            labelConfirmPass.Size = new Size(153, 29);
            labelConfirmPass.TabIndex = 8;
            labelConfirmPass.Text = "Confirm Password";
            // 
            // textBoxConfirmPass
            // 
            textBoxConfirmPass.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxConfirmPass.Location = new Point(49, 294);
            textBoxConfirmPass.Name = "textBoxConfirmPass";
            textBoxConfirmPass.Size = new Size(436, 33);
            textBoxConfirmPass.TabIndex = 7;
            // 
            // labelBusinessName
            // 
            labelBusinessName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelBusinessName.AutoSize = true;
            labelBusinessName.Location = new Point(49, 357);
            labelBusinessName.Name = "labelBusinessName";
            labelBusinessName.Size = new Size(128, 29);
            labelBusinessName.TabIndex = 6;
            labelBusinessName.Text = "Business Name";
            // 
            // textBoxBusinessName
            // 
            textBoxBusinessName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxBusinessName.Location = new Point(49, 386);
            textBoxBusinessName.Name = "textBoxBusinessName";
            textBoxBusinessName.Size = new Size(436, 33);
            textBoxBusinessName.TabIndex = 5;
            // 
            // labelPassword
            // 
            labelPassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(49, 175);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(86, 29);
            labelPassword.TabIndex = 4;
            labelPassword.Text = "Password";
            // 
            // labelEmail
            // 
            labelEmail.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelEmail.AutoSize = true;
            labelEmail.Location = new Point(49, 82);
            labelEmail.Name = "labelEmail";
            labelEmail.Size = new Size(58, 29);
            labelEmail.TabIndex = 2;
            labelEmail.Text = "Email";
            // 
            // labelCreateAcc
            // 
            labelCreateAcc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelCreateAcc.AutoSize = true;
            labelCreateAcc.Font = new Font("Sitka Banner", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelCreateAcc.Location = new Point(180, 20);
            labelCreateAcc.Name = "labelCreateAcc";
            labelCreateAcc.Size = new Size(223, 26);
            labelCreateAcc.TabIndex = 1;
            labelCreateAcc.Text = "Create Your Account for Free";
            // 
            // textBoxEmail
            // 
            textBoxEmail.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxEmail.Location = new Point(50, 111);
            textBoxEmail.Name = "textBoxEmail";
            textBoxEmail.Size = new Size(436, 33);
            textBoxEmail.TabIndex = 0;
            textBoxEmail.TextChanged += textBoxEmail_TextChanged;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom;
            panel2.BackColor = Color.Transparent;
            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(pictureBox1);
            panel2.Location = new Point(403, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(474, 69);
            panel2.TabIndex = 2;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.None;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox2.Location = new Point(126, -6);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(293, 95);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = Properties.Resources.Logo2;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(60, -10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(71, 79);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackgroundImage = Properties.Resources._5;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1280, 800);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form2";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form2";
            WindowState = FormWindowState.Minimized;
            SizeChanged += Form2_SizeChanged;
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panelGlowWrapper.ResumeLayout(false);
            panelCreateAccCard.ResumeLayout(false);
            panelCreateAccCard.PerformLayout();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private Button btnMaximize;
        private Button btnClose;
        private Button btnMinimize;
        private Panel panelGlowWrapper;
        private Panel panelCreateAccCard;
        private Panel panel2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private Label labelEmail;
        private Label labelCreateAcc;
        private TextBox textBoxEmail;
        private TextBox textBoxPassword;
        private Label labelBusinessName;
        private TextBox textBoxBusinessName;
        private Label labelPassword;
        private LinkLabel linkLabelGoToLogin;
        private Label labelGoToLogin;
        private Button btnSignup;
        private Label labelConfirmPass;
        private TextBox textBoxConfirmPass;
        private Button btnTogglePassword;
        private Button btnToggleConfirmPass;
    }
}