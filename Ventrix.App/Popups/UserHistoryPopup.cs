using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public partial class UserHistoryPopup : MaterialForm
    {
        private readonly string _schoolId;
        private readonly BorrowService _borrowService;
        private DataGridView dgvUserHistory;

        public UserHistoryPopup(string schoolId, string studentName, BorrowService borrowService)
        {
            _schoolId = schoolId;
            _borrowService = borrowService;

            InitializeComponent();
            this.Text = $"Full Audit History: {studentName} ({schoolId})";
            this.Size = new Size(800, 500);

            SetupGrid();
            LoadStudentHistory();
        }

        private void SetupGrid()
        {
            dgvUserHistory = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(this.Width - 40, this.Height - 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 235, 240)
            };

            dgvUserHistory.ColumnHeadersHeight = 40;
            dgvUserHistory.EnableHeadersVisualStyles = false;
            dgvUserHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(13, 71, 161);
            dgvUserHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUserHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUserHistory.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvUserHistory.RowTemplate.Height = 40;

            dgvUserHistory.Columns.Add("Id", "Record ID");
            dgvUserHistory.Columns.Add("Item", "Item Name");
            dgvUserHistory.Columns.Add("Borrowed", "Date Borrowed");
            dgvUserHistory.Columns.Add("Returned", "Date Returned");
            dgvUserHistory.Columns.Add("Status", "Status");

            this.Controls.Add(dgvUserHistory);

            Button btnClose = new Button { Text = "CLOSE WINDOW", Location = new Point(this.Width / 2 - 75, this.Height - 60), Width = 150, Height = 35, BackColor = Color.White, ForeColor = Color.FromArgb(13, 71, 161), FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.Bottom };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private async void LoadStudentHistory()
        {
            var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
            var studentRecords = allRecords.Where(b => b.BorrowerId == _schoolId).OrderByDescending(b => b.BorrowDate).ToList();

            foreach (var r in studentRecords)
            {
                string rStamp = r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString("MMM dd, yyyy - hh:mm tt") : "---";
                dgvUserHistory.Rows.Add(r.Id, r.ItemName, r.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt"), rStamp, r.Status.ToString());
            }

            // Color code the status
            foreach (DataGridViewRow row in dgvUserHistory.Rows)
            {
                if (row.Cells["Status"].Value.ToString() == nameof(BorrowStatus.Active))
                    row.Cells["Status"].Style.ForeColor = Color.DarkOrange;
                else
                    row.Cells["Status"].Style.ForeColor = Color.MediumSeaGreen;
            }
        }
    }
}