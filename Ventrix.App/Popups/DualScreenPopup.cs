using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Popups
{
    /// <summary>
    /// Modal dialog displayed immediately after the admin logs in.
    /// Prompts the admin to connect a second monitor (via HDMI or DisplayPort)
    /// so the Borrower Portal can be displayed on it while the Admin Dashboard
    /// runs on the primary display.
    /// </summary>
    public class DualScreenPopup : Form
    {
        // ─── Colours (matches ThemeManager / BorrowerPortal palette) ─────────
        private static readonly Color VentrixBlue = Color.FromArgb(13, 71, 161);
        private static readonly Color VentrixLightBlue = Color.FromArgb(33, 150, 243);
        private static readonly Color SurfaceGray = Color.FromArgb(243, 244, 246);
        private static readonly Color BorderGray = Color.FromArgb(209, 213, 219);
        private static readonly Color TextDark = Color.FromArgb(31, 41, 55);
        private static readonly Color TextMuted = Color.FromArgb(107, 114, 128);
        private static readonly Color SuccessGreen = Color.FromArgb(16, 185, 129);
        private static readonly Color WarningAmber = Color.FromArgb(245, 158, 11);

        // ─── Controls ─────────────────────────────────────────────────────────
        private Label _lblScreenCount = null!;
        private Label _lblStatusIcon = null!;
        private Label _lblStatusText = null!;
        private Button _btnScan = null!;
        private Button _btnUseSecond = null!;
        private Button _btnSkip = null!;
        private Panel _statusPanel = null!;

        private readonly DualScreenService _service;

        // ─── Constructor ──────────────────────────────────────────────────────

        public DualScreenPopup(DualScreenService service)
        {
            _service = service;

            // ── Form chrome ─────────────────────────────────────────────────
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(500, 440);
            BackColor = Color.White;
            ShowInTaskbar = false;
            TopMost = true;

            // Rounded corners (same technique as ToastNotification)
            var elipse = new Guna2Elipse { TargetControl = this, BorderRadius = 16 };

            BuildUI();
            RefreshScreenStatus();
        }

        // ─── UI Construction ──────────────────────────────────────────────────

        private void BuildUI()
        {
            // ── Header panel ────────────────────────────────────────────────
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = VentrixBlue
            };

            var headerIcon = new Label
            {
                Text = "🖥️",
                Font = new Font("Segoe UI", 22F),
                ForeColor = Color.White,
                Size = new Size(54, 72),
                Location = new Point(20, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var headerTitle = new Label
            {
                Text = "Borrower Screen Setup",
                Font = new Font("Segoe UI Semibold", 15F),
                ForeColor = Color.White,
                Size = new Size(380, 40),
                Location = new Point(78, 16),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var headerSub = new Label
            {
                Text = "Multi-screen mode",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(180, 210, 255),
                Size = new Size(380, 18),
                Location = new Point(79, 50),
                TextAlign = ContentAlignment.MiddleLeft
            };

            header.Controls.AddRange(new Control[] { headerIcon, headerTitle, headerSub });

            // ── Description ─────────────────────────────────────────────────
            var lblDescription = new Label
            {
                Text = "Connect a second monitor to this computer via HDMI or DisplayPort. " +
                            "Ventrix will display the Borrower Portal on that screen so students " +
                            "can type in their details directly while you manage the dashboard here.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = TextDark,
                Size = new Size(440, 70),
                Location = new Point(30, 88),
                TextAlign = ContentAlignment.TopLeft
            };

            // ── Status card ─────────────────────────────────────────────────
            _statusPanel = new Panel
            {
                Size = new Size(440, 80),
                Location = new Point(30, 166),
                BackColor = SurfaceGray,
                Padding = new Padding(16, 0, 16, 0)
            };

            // Rounded corners on the status card
            new Guna2Elipse { TargetControl = _statusPanel, BorderRadius = 10 };

            _lblStatusIcon = new Label
            {
                Font = new Font("Segoe UI", 20F),
                Size = new Size(44, 80),
                Location = new Point(16, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _lblScreenCount = new Label
            {
                Font = new Font("Segoe UI Semibold", 13F),
                ForeColor = TextDark,
                Size = new Size(250, 36),
                Location = new Point(68, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblStatusText = new Label
            {
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextMuted,
                Size = new Size(340, 28),
                Location = new Point(68, 44),
                TextAlign = ContentAlignment.TopLeft
            };

            _statusPanel.Controls.AddRange(new Control[]
            {
                _lblStatusIcon, _lblScreenCount, _lblStatusText
            });

            // ── Scan button ──────────────────────────────────────────────────
            _btnScan = new Button
            {
                Text = "🔍  Scan for Monitor",
                Font = new Font("Segoe UI Semibold", 10F),
                Size = new Size(440, 40),
                Location = new Point(30, 260),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = VentrixBlue,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _btnScan.FlatAppearance.BorderColor = VentrixBlue;
            _btnScan.FlatAppearance.BorderSize = 2;
            _btnScan.Click += (_, _) => RefreshScreenStatus();

            // ── Divider ──────────────────────────────────────────────────────
            var divider = new Panel
            {
                Size = new Size(440, 1),
                Location = new Point(30, 318),
                BackColor = BorderGray
            };

            // ── Action buttons ───────────────────────────────────────────────
            _btnUseSecond = new Button
            {
                Text = "Use Second Screen",
                Font = new Font("Segoe UI Semibold", 10F),
                Size = new Size(210, 44),
                Location = new Point(30, 332),
                FlatStyle = FlatStyle.Flat,
                BackColor = VentrixBlue,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _btnUseSecond.FlatAppearance.BorderSize = 0;
            _btnUseSecond.Click += BtnUseSecond_Click;

            _btnSkip = new Button
            {
                Text = "Skip for Now",
                Font = new Font("Segoe UI Semibold", 10F),
                Size = new Size(210, 44),
                Location = new Point(260, 332),
                FlatStyle = FlatStyle.Flat,
                BackColor = SurfaceGray,
                ForeColor = TextMuted,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _btnSkip.FlatAppearance.BorderColor = BorderGray;
            _btnSkip.FlatAppearance.BorderSize = 1;
            _btnSkip.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            // ── Hint label ───────────────────────────────────────────────────
            var lblHint = new Label
            {
                Text = "Connect a monitor via HDMI or DisplayPort, then click Scan to detect it.",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = TextMuted,
                Size = new Size(440, 22),
                Location = new Point(30, 392),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ── Hover effects ────────────────────────────────────────────────
            AddHover(_btnScan,
                hoverBack: Color.FromArgb(235, 242, 255),
                normalBack: Color.White,
                hoverFore: VentrixBlue,
                normalFore: VentrixBlue);

            AddHover(_btnUseSecond,
                hoverBack: Color.FromArgb(10, 50, 120),
                normalBack: VentrixBlue,
                hoverFore: Color.White,
                normalFore: Color.White);

            AddHover(_btnSkip,
                hoverBack: BorderGray,
                normalBack: SurfaceGray,
                hoverFore: TextDark,
                normalFore: TextMuted);

            // ── Assemble ─────────────────────────────────────────────────────
            Controls.AddRange(new Control[]
            {
                header,
                lblDescription,
                _statusPanel,
                _btnScan,
                divider,
                _btnUseSecond,
                _btnSkip,
                lblHint
            });
        }

        // ─── Logic ────────────────────────────────────────────────────────────

        /// <summary>Polls <see cref="DualScreenService"/> and updates the status card.</summary>
        private void RefreshScreenStatus()
        {
            int count = _service.ScreenCount();
            bool found = _service.HasSecondScreen();

            if (found)
            {
                _lblStatusIcon.Text = "🖥️";
                _lblScreenCount.Text = $"Screens detected: {count}";
                _lblScreenCount.ForeColor = SuccessGreen;
                _lblStatusText.Text = "A second screen is ready. Click \"Use Second Screen\" to continue.";
                _statusPanel.BackColor = Color.FromArgb(236, 253, 245); // light green tint
            }
            else
            {
                _lblStatusIcon.Text = "⚠️";
                _lblScreenCount.Text = "Screens detected: 1";
                _lblScreenCount.ForeColor = WarningAmber;
                _lblStatusText.Text = "Only one screen found. Plug in your monitor, then click Scan.";
                _statusPanel.BackColor = Color.FromArgb(255, 251, 235); // light amber tint
            }

            _btnUseSecond.Enabled = found;
            _btnUseSecond.BackColor = found ? VentrixBlue : Color.FromArgb(180, 180, 180);
            _btnUseSecond.Cursor = found ? Cursors.Hand : Cursors.Default;
        }

        private void BtnUseSecond_Click(object? sender, EventArgs e)
        {
            // Re-check in case screen was plugged in between last scan and click
            if (!_service.HasSecondScreen())
            {
                RefreshScreenStatus();
                MessageBox.Show(
                    "No second monitor detected yet.\n\nPlease connect your monitor via HDMI or DisplayPort, then click Scan.",
                    "Monitor Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private static void AddHover(Button btn,
            Color hoverBack, Color normalBack,
            Color hoverFore, Color normalFore)
        {
            btn.MouseEnter += (_, _) => { btn.BackColor = hoverBack; btn.ForeColor = hoverFore; };
            btn.MouseLeave += (_, _) => { btn.BackColor = normalBack; btn.ForeColor = normalFore; };
        }

        // Allow Escape key to skip
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}