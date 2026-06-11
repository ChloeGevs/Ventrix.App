using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    /// <summary>
    /// Manages multi-screen detection and window placement for the Borrower Tablet feature.
    /// Allows the Borrower Portal to be displayed on a connected second screen (e.g. a tablet
    /// or secondary monitor) while the Admin Dashboard remains on the primary screen.
    /// </summary>
    public class DualScreenService
    {
        private const int DefaultPortalWidth = 1200;
        private const int DefaultPortalHeight = 800;

        // ─── Detection ───────────────────────────────────────────────────────────

        /// <summary>Returns true when more than one physical screen is connected.</summary>
        public bool HasSecondScreen() => Screen.AllScreens.Length > 1;

        /// <summary>Returns the number of currently connected screens.</summary>
        public int ScreenCount() => Screen.AllScreens.Length;

        /// <summary>
        /// Returns the first non-primary screen, or null when only one screen is connected.
        /// </summary>
        public Screen? GetSecondaryScreen()
        {
            foreach (var screen in Screen.AllScreens)
            {
                if (!screen.Primary)
                    return screen;
            }
            return null;
        }

        /// ─── Positioning ─────────────────────────────────────────────────────────

        /// <summary>
        /// Moves and resizes <paramref name="form"/> to fill the target screen completely,
        /// removing borders and locking it on top.
        /// </summary>
        public void PositionFormOnScreen(Form form, Screen targetScreen)
        {
            // 1. Strip the borders and title bar so it cannot be dragged or minimized
            form.FormBorderStyle = FormBorderStyle.None;

            // 2. Force it to sit above the Windows Taskbar and all other applications
            form.TopMost = true;

            // 3. Position it using .Bounds instead of .WorkingArea so it completely covers the taskbar
            form.WindowState = FormWindowState.Normal;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = targetScreen.Bounds.Location;
            form.Size = targetScreen.Bounds.Size;
        }

        /// <summary>
        /// Returns <paramref name="form"/> to the centre of the primary screen,
        /// restoring its title bar and default behavior.
        /// </summary>
        public void ReturnFormToPrimaryScreen(Form form)
        {
            Screen primary = Screen.PrimaryScreen ?? Screen.AllScreens[0];

            // 1. Restore the borders and allow other windows to go over it
            form.FormBorderStyle = FormBorderStyle.Sizable; // Or FixedSingle, depending on your original design
            form.TopMost = false;

            form.WindowState = FormWindowState.Normal;
            form.StartPosition = FormStartPosition.Manual;

            Rectangle wa = primary.WorkingArea;
            int w = Math.Min(DefaultPortalWidth, wa.Width);
            int h = Math.Min(DefaultPortalHeight, wa.Height);

            form.Size = new Size(w, h);
            form.Location = new Point(
                wa.Left + (wa.Width - w) / 2,
                wa.Top + (wa.Height - h) / 2
            );
        }
    }
}