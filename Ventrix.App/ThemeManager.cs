using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Ventrix.App
{
    public static class ThemeManager
    {
        // --- Vibrant Modern SaaS / Gemini Palette ---
        public static Color VentrixBlue = Color.FromArgb(79, 70, 229);      // Modern Deep Indigo-Blue
        public static Color VentrixLightBlue = Color.FromArgb(99, 102, 241); // Vibrant Hover Blue

        public static Color CanvasBackground = Color.FromArgb(248, 250, 252); // Ultra-clean slate-tinted canvas
        public static Color SurfaceColor = Color.White;                       // Crisp white cards
        public static Color BorderColor = Color.FromArgb(226, 232, 240);      // Soft, refined border gray

        public static Color TextPrimary = Color.FromArgb(15, 23, 42);         // Deep slate dark text for maximum clarity
        public static Color TextSecondary = Color.FromArgb(100, 116, 139);    // Muted cool gray text

        public static Color AccentBlue = Color.FromArgb(79, 70, 229);         // Primary interactive indigo
        public static Color AccentBlueHover = Color.FromArgb(67, 56, 202);    // Deepened hover state

        public static Font PrimaryFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font HeaderFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static Font ButtonFont = new Font("Segoe UI", 10F, FontStyle.Bold);

        // --- Legacy Support Methods ---
        public static void Initialize(Control control = null) { }
        public static void ApplyMaterialTheme(Control control) { }

        public static void ApplyTheme(Control control)
        {
            if (control != null)
            {
                control.BackColor = CanvasBackground;
                control.ForeColor = TextPrimary;
            }
        }

        public static void ApplyCustomFont(Control control, Font font, Color color)
        {
            if (control != null)
            {
                control.Font = font;
                control.ForeColor = color;
            }
        }

        public static void FixTransparency(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label || control is PictureBox)
                {
                    control.BackColor = Color.Transparent;
                }

                if (control.HasChildren)
                {
                    FixTransparency(control);
                }
            }
        }

        // --- ANIMATION & TRANSITION UTILITIES ---

        /// <summary>
        /// Smoothly fades in a control or panel using Guna2Transition for modern motion design.
        /// </summary>
        public static void FadeInControl(Guna2Transition transition, Control control)
        {
            if (transition != null && control != null)
            {
                control.Visible = false;
                transition.ShowSync(control);
            }
        }

        /// <summary>
        /// Smoothly transitions between two panels (e.g., swapping dashboard views).
        /// </summary>
        public static void SwapViews(Guna2Transition transition, Control controlToHide, Control controlToShow)
        {
            if (transition != null)
            {
                transition.HideSync(controlToHide);
                controlToShow.Visible = false;
                transition.ShowSync(controlToShow);
            }
            else
            {
                controlToHide.Visible = false;
                controlToShow.Visible = true;
            }
        }

        // --- MODERN UI COMPONENTS ---

        public static void StyleCard(Guna2Panel panel)
        {
            panel.BackColor = Color.Transparent;
            panel.FillColor = SurfaceColor;
            panel.BorderRadius = 14; // Smooth, modern large rounding
            panel.BorderColor = BorderColor;
            panel.BorderThickness = 1;
            panel.ShadowDecoration.Enabled = false;
        }

        public static void StyleCard(Control cardControl)
        {
            if (cardControl != null)
            {
                cardControl.BackColor = SurfaceColor;
            }
        }

        public static void StyleDataGrid(Guna2DataGridView grid)
        {
            grid.BackgroundColor = SurfaceColor;
            grid.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(241, 245, 249); // Soft slate header tint
            grid.ThemeStyle.HeaderStyle.ForeColor = TextPrimary;
            grid.ColumnHeadersHeight = 44;

            grid.ThemeStyle.RowsStyle.Font = PrimaryFont;
            grid.ThemeStyle.RowsStyle.BackColor = SurfaceColor;
            grid.ThemeStyle.RowsStyle.ForeColor = TextPrimary;
            grid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(224, 231, 255); // Soft indigo-tinted selection
            grid.ThemeStyle.RowsStyle.SelectionForeColor = VentrixBlue;

            grid.GridColor = Color.FromArgb(241, 245, 249);
            grid.RowTemplate.Height = 50; // Airy, spacious modern spacing
            grid.AllowUserToResizeRows = false;
            grid.BorderStyle = BorderStyle.None;
        }

        public static void StyleActionButton(Guna2Button btn, Color defaultColor, Color hoverColor)
        {
            btn.BorderRadius = 10; // Modern pill/rounded rectangle look
            btn.Animated = true;   // Enables Guna's built-in ripple/hover click animations
            btn.Font = ButtonFont;
            btn.FillColor = defaultColor;
            btn.HoverState.FillColor = hoverColor;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.ShadowDecoration.Enabled = false;
        }

        public static void StyleSidebarButton(Guna2Button btn)
        {
            btn.BorderRadius = 8;  // Gives sidebar buttons a clean floating pill appearance
            btn.Animated = true;
            btn.Font = ButtonFont;
            btn.FillColor = Color.Transparent;
            btn.ForeColor = Color.White;
            btn.HoverState.FillColor = Color.FromArgb(30, 255, 255, 255); // Smooth glassmorphism hover effect
            btn.CheckedState.FillColor = SurfaceColor;
            btn.CheckedState.ForeColor = VentrixBlue;
            btn.Cursor = Cursors.Hand;
            btn.ShadowDecoration.Enabled = false;
        }

        public static void StyleTextBox(Guna2TextBox txt)
        {
            txt.BorderRadius = 10;
            txt.FillColor = Color.FromArgb(248, 250, 252);
            txt.BorderColor = BorderColor;
            txt.FocusedState.BorderColor = AccentBlue;
            txt.HoverState.BorderColor = AccentBlue;
            txt.Font = PrimaryFont;
            txt.ForeColor = TextPrimary;
        }

        public static void StylePopup(Form popupForm, Guna2BorderlessForm borderlessForm = null, Guna2AnimateWindow animateWindow = null)
        {
            popupForm.BackColor = SurfaceColor;
            popupForm.StartPosition = FormStartPosition.CenterParent;
            popupForm.FormBorderStyle = FormBorderStyle.None;

            if (borderlessForm != null)
            {
                borderlessForm.BorderRadius = 16;
                borderlessForm.ContainerControl = popupForm;
                borderlessForm.ShadowColor = Color.FromArgb(120, 0, 0, 0); // Softer, deeper atmospheric shadow
            }

            if (animateWindow != null)
            {
                animateWindow.AnimationType = Guna2AnimateWindow.AnimateWindowType.AW_BLEND;
                animateWindow.Interval = 200; // Smooth 200ms fade-in speed
                animateWindow.TargetForm = popupForm;
            }

            FixTransparency(popupForm);
        }
    }
}