using System.Runtime.InteropServices;

namespace 
{
    public partial class Form1 : Form
    {
        // --- Windows API Imports ---
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        // --- Flicker Fix and Drop Shadow ---
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_COMPOSITED = 0x02000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.MinimumSize = new Size(650, 750);

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            var preference = 2; 
            DwmSetWindowAttribute(this.Handle, 33, ref preference, sizeof(int));

            tableLayoutPanel1.BackColor = Color.Transparent;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelLoginCard.BorderStyle = BorderStyle.FixedSingle;
            panelLoginCard.BackColor = Color.White;
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.Dock = DockStyle.Fill;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;

            panelUserWrapper.BackColor = Color.FromArgb(30, 0, 0, 0);
            panelPassWrapper.BackColor = Color.FromArgb(30, 0, 0, 0);

            SendMessage(textBoxName.Handle, 0x1501, 0, "Username or Email");
            SendMessage(textBoxPassword.Handle, 0x1501, 0, "Enter Password");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximized_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            this.WindowState = (this.WindowState == FormWindowState.Normal)
                               ? FormWindowState.Maximized
                               : FormWindowState.Normal;

            tableLayoutPanel1.PerformLayout();

            this.ResumeLayout(true);
            this.Invalidate(true); 
            this.Refresh();
        }

        private void btnMinimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            tableLayoutPanel1.BackColor = Color.Transparent;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            this.SuspendLayout();

            if (this.WindowState == FormWindowState.Normal)
            {
               
                this.CenterToScreen();
            }

            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.PerformLayout();

            this.ResumeLayout(true);
            this.Invalidate();
            this.Update();

            panelLoginCard.Invalidate();
            panelLoginCard.Update();
        }

        private void textBoxName_Enter(object sender, EventArgs e)
        {
            panelUnderline1.BackColor = Color.FromArgb(24, 119, 242);
            panelUnderline1.Height = 2;
        }

        private void textBoxName_Leave(object sender, EventArgs e)
        {
            panelUnderline1.BackColor = Color.LightGray;
            panelUnderline1.Height = 1;
        }

        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            panelUnderline2.BackColor = Color.FromArgb(24, 119, 242);
            panelUnderline2.Height = 2;
        }

        private void textBoxPassword_Leave(object sender, EventArgs e)
        {
            panelUnderline2.BackColor = Color.LightGray;
            panelUnderline2.Height = 1;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = !textBoxPassword.UseSystemPasswordChar;
        }

        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }
        
    }
}