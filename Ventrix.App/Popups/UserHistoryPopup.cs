using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Guna.UI2.WinForms;

namespace Ventrix.App.Popups
{
    public partial class UserHistoryPopup : Form
    {
        private readonly string _schoolId;
        private readonly BorrowService _borrowService;
        private readonly DateTime? _startDate;
        private readonly DateTime? _endDate;
        private readonly string _studentName;

        private int _targetY;
        private bool _isAnimating = false;
        private double _animStep = 0.0;
        private Form _dimOverlay;

        // Typewriter Effect Variables
        private string _targetTitleText = "";
        private int _typewriterIndex = 0;

        public UserHistoryPopup(string schoolId, string studentName, BorrowService borrowService, DateTime? startDate = null, DateTime? endDate = null)
        {
            _schoolId = schoolId;
            _studentName = studentName;
            _borrowService = borrowService;
            _startDate = startDate;
            _endDate = endDate;

            InitializeComponent();

            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;

            if (_startDate.HasValue && _endDate.HasValue)
                _targetTitleText = $"Audit History: {_studentName} ({_startDate.Value:MMM dd} to {_endDate.Value:MMM dd})";
            else
                _targetTitleText = $"Full Audit History: {_studentName} ({_schoolId})";

            lblTitle.Text = "";

            this.Load += Popup_Load;
            this.FormClosed += Popup_FormClosed;

            SetupGrid();
            LoadStudentHistory();
        }

        private void ShowDimOverlay(Form parentForm)
        {
            if (parentForm == null) return;

            _dimOverlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Bounds = parentForm.Bounds,
                BackColor = Color.Black,
                Opacity = 0.0,
                ShowInTaskbar = false,
                Owner = parentForm
            };

            System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer { Interval = 10 };
            fadeTimer.Tick += (s, e) =>
            {
                if (_dimOverlay.Opacity < 0.45)
                {
                    _dimOverlay.Opacity += 0.05;
                }
                else
                {
                    fadeTimer.Stop();
                    fadeTimer.Dispose();
                }
            };

            _dimOverlay.Show(parentForm);
            fadeTimer.Start();
        }

        private void Popup_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                ShowDimOverlay(this.Owner);
            }

            _targetY = this.Location.Y;
            this.Location = new Point(this.Location.X, _targetY + 25);

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 15 };
            animTimer.Tick += (s, ev) =>
            {
                _animStep += 0.12;

                if (this.Opacity < 1.0) this.Opacity += 0.15;

                double progress = Math.Min(1.0, _animStep);
                double easeOutBack = 1 + (--progress) * progress * (2.7 * progress + 1.7);

                int currentY = _targetY + 25 - (int)(25 * easeOutBack);
                this.Location = new Point(this.Location.X, currentY);

                if (_animStep >= 1.0 && this.Opacity >= 0.98)
                {
                    this.Opacity = 1.0;
                    this.Location = new Point(this.Location.X, _targetY);
                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };
            animTimer.Start();

            System.Windows.Forms.Timer typeTimer = new System.Windows.Forms.Timer { Interval = 30 };
            typeTimer.Tick += (s, ev) =>
            {
                if (_typewriterIndex < _targetTitleText.Length)
                {
                    lblTitle.Text += _targetTitleText[_typewriterIndex];
                    _typewriterIndex++;
                }
                else
                {
                    typeTimer.Stop();
                    typeTimer.Dispose();
                }
            };

            Task.Delay(150).ContinueWith(t => this.Invoke(new Action(() => typeTimer.Start())));
        }

        private void Popup_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_dimOverlay != null && !_dimOverlay.IsDisposed)
            {
                _dimOverlay.Close();
                _dimOverlay.Dispose();
            }
        }

        private void SetupGrid()
        {
            dgvUserHistory.Location = new Point(22, 65);
            dgvUserHistory.Size = new Size(this.Width - 44, 195); // Compact grid height to fit snug
            dgvUserHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvUserHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUserHistory.AllowUserToAddRows = false;
            dgvUserHistory.ReadOnly = true;
            dgvUserHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUserHistory.RowHeadersVisible = false;
            dgvUserHistory.BackgroundColor = Color.White;
            dgvUserHistory.BorderStyle = BorderStyle.None;
            dgvUserHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvUserHistory.GridColor = Color.FromArgb(241, 245, 249);

            dgvUserHistory.ColumnHeadersHeight = 38;
            dgvUserHistory.EnableHeadersVisualStyles = false;

            dgvUserHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvUserHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            dgvUserHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dgvUserHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            dgvUserHistory.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(71, 85, 105);

            dgvUserHistory.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvUserHistory.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgvUserHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 245, 249);
            dgvUserHistory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvUserHistory.RowTemplate.Height = 36;

            var colId = new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "Record ID", FillWeight = 50 };
            var colItem = new DataGridViewTextBoxColumn { Name = "Item", HeaderText = "Item Name", FillWeight = 110 };
            var colBorrowed = new DataGridViewTextBoxColumn { Name = "Borrowed", HeaderText = "Date Borrowed", FillWeight = 110 };
            var colReturned = new DataGridViewTextBoxColumn { Name = "Returned", HeaderText = "Date Returned", FillWeight = 110 };
            var colStatus = new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", FillWeight = 70 };

            dgvUserHistory.Columns.AddRange(new DataGridViewColumn[] { colId, colItem, colBorrowed, colReturned, colStatus });
        }

        private async void LoadStudentHistory()
        {
            var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
            var studentQuery = allRecords.Where(b => b.BorrowerId == _schoolId);

            if (_startDate.HasValue && _endDate.HasValue)
            {
                DateTime endOfDay = _endDate.Value.Date.AddDays(1).AddTicks(-1);
                studentQuery = studentQuery.Where(b => b.BorrowDate >= _startDate.Value.Date && b.BorrowDate <= endOfDay);
            }

            var finalRecords = studentQuery.OrderByDescending(b => b.BorrowDate).ToList();

            foreach (var r in finalRecords)
            {
                string rStamp = r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString("MMM dd, yyyy - hh:mm tt") : "---";
                dgvUserHistory.Rows.Add(r.Id, r.ItemName, r.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt"), rStamp, r.Status.ToString());
            }

            foreach (DataGridViewRow row in dgvUserHistory.Rows)
            {
                string statusStr = row.Cells["Status"].Value?.ToString();
                if (statusStr == nameof(BorrowStatus.Active))
                {
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(217, 119, 6);
                }
                else if (statusStr == nameof(BorrowStatus.Overdue))
                {
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(239, 68, 68);
                }
                else if (statusStr == "PendingReturn")
                {
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(14, 165, 233);
                }
                else
                {
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(16, 185, 129);
                }
            }
        }

        private void CloseAnimated()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            if (_dimOverlay != null && !_dimOverlay.IsDisposed)
            {
                System.Windows.Forms.Timer overlayFade = new System.Windows.Forms.Timer { Interval = 10 };
                overlayFade.Tick += (s, ev) =>
                {
                    if (_dimOverlay.Opacity > 0) _dimOverlay.Opacity -= 0.05;
                    else { overlayFade.Stop(); overlayFade.Dispose(); }
                };
                overlayFade.Start();
            }

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (s, ev) =>
            {
                this.Opacity -= 0.15;
                this.Location = new Point(this.Location.X, this.Location.Y + 4);

                if (this.Opacity <= 0)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };
            animTimer.Start();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            CloseAnimated();
        }
    }
}