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

        // ─── Positioning ─────────────────────────────────────────────────────────

        /// <summary>
        /// Moves and resizes <paramref name="form"/> to fill the working area of
        /// <paramref name="targetScreen"/>. Ideal for tablet / kiosk display.
        /// </summary>
        public void PositionFormOnScreen(Form form, Screen targetScreen)
        {
            // Normalise first so we can reposition freely
            form.WindowState = FormWindowState.Normal;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = targetScreen.WorkingArea.Location;
            form.Size = targetScreen.WorkingArea.Size;
        }

        /// <summary>
        /// Returns <paramref name="form"/> to the centre of the primary screen at
        /// its original 1200 × 800 resolution (capped to the available working area).
        /// </summary>
        public void ReturnFormToPrimaryScreen(Form form)
        {
            Screen primary = Screen.PrimaryScreen ?? Screen.AllScreens[0];

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