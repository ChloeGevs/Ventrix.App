using MaterialSkin;
using MaterialSkin.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    public static class ThemeManager
    {
        public static readonly Color VentrixBlue = Color.FromArgb(13, 71, 161);
        public static readonly Color VentrixLightBlue = Color.FromArgb(33, 150, 243);

        // In ThemeManager.cs
        public static readonly Font HeaderFont = new Font("Segoe UI", 20F, FontStyle.Bold);

        // Use "Segoe UI Semibold" as the string, and FontStyle.Regular
        public static readonly Font SubHeaderFont = new Font("Segoe UI Semibold", 12F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);

        public static void Initialize(MaterialForm form)
        {
            var manager = MaterialSkinManager.Instance;

            manager.EnforceBackcolorOnAllComponents = false;

            manager.AddFormToManage(form);

            if (manager.Theme != MaterialSkinManager.Themes.LIGHT)
            {
                manager.Theme = MaterialSkinManager.Themes.LIGHT;
                manager.ColorScheme = new ColorScheme(
                    VentrixBlue,
                    Color.FromArgb(10, 50, 120),
                    VentrixLightBlue,
                    Color.FromArgb(30, 136, 229),
                    TextShade.WHITE);
            }
        }
        public static void ApplyCustomFont(Control ctrl, Font font, Color? color = null)
        {
            ctrl.Font = font;
            if (color.HasValue) ctrl.ForeColor = color.Value;

            if (ctrl is MaterialLabel mLabel)
            {
                mLabel.UseAccent = false;
            }

            ctrl.Tag = "LockedFont";
        }
    }
}