using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public class MultiUserSelectionPopup : Form
    {
        public List<User> SelectedUsers { get; private set; } = new List<User>();
        private CheckedListBox chkListUsers;

        public MultiUserSelectionPopup(string title, string instruction, List<User> users)
        {
            this.Text = title;
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.Black, Location = new Point(20, 20), AutoSize = true });
            Controls.Add(new Label { Text = instruction, Font = new Font("Segoe UI", 10F), ForeColor = Color.Gray, Location = new Point(20, 55), Size = new Size(390, 40) });

            chkListUsers = new CheckedListBox { Location = new Point(20, 100), Size = new Size(390, 280), Font = new Font("Segoe UI", 10F), CheckOnClick = true };

            foreach (var user in users.Where(u => u.Role != UserRole.Admin))
            {
                chkListUsers.Items.Add(new UserWrapper { DisplayText = $"[{user.UserId}] {user.FullName}", User = user });
            }
            Controls.Add(chkListUsers);

            Button btnConfirm = new Button { Text = "Generate", BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(230, 400), Size = new Size(180, 40) };
            btnConfirm.Click += (s, e) => {
                SelectedUsers.AddRange(chkListUsers.CheckedItems.Cast<UserWrapper>().Select(w => w.User));
                if (!SelectedUsers.Any()) { MessageBox.Show("Select at least one user."); return; }
                this.DialogResult = DialogResult.OK; this.Close();
            };
            Controls.Add(btnConfirm);

            Button btnCancel = new Button { Text = "Cancel", BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Location = new Point(20, 400), Size = new Size(100, 40) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            Controls.Add(btnCancel);
        }

        private class UserWrapper { public string DisplayText { get; set; } public User User { get; set; } public override string ToString() => DisplayText; }
    }
}