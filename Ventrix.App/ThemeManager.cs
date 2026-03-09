using Guna.UI2.WinForms;
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

        public static readonly Font HeaderFont = new Font("Segoe UI", 20F, FontStyle.Bold);

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

        public static void ApplyMaterialTheme(MaterialSkin.Controls.MaterialForm form)
        {
            var manager = MaterialSkinManager.Instance;
            manager.AddFormToManage(form);
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(
                Color.FromArgb(13, 71, 161), 
                Color.FromArgb(10, 50, 120),  
                Color.FromArgb(33, 150, 243), 
                Color.FromArgb(30, 136, 229), 
                TextShade.WHITE
            );

            Guna2Elipse elipse = new Guna2Elipse();
            elipse.TargetControl = form;
            elipse.BorderRadius = 20;
        }
    }
}