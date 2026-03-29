using ClosedXML.Excel;
using MaterialSkin.Controls; // 1. Added this to access MaterialForm
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Popups // 2. Updated to match your project's actual namespace
{
    // 3. Changed from ': Form' to ': MaterialForm'
    public partial class BorrowerItemsPopup : MaterialForm
    {
        public BorrowerItemsPopup(string borrowerName, string status, string itemsString)
        {
            InitializeComponent();

            // 4. Apply your project's custom theme (this provides the automatic faded background!)
            ThemeManager.ApplyMaterialTheme(this);

            // 1. Set the dynamic texts
            lblTitle.Text = $"Items for {borrowerName}";

            // 2. Set the helper text and color based on the status
            if (status == "Pending")
            {
                lblHelper.Text = "STATUS: PENDING • Please locate these items for approval:";
                lblHelper.ForeColor = Color.Goldenrod;
            }
            else if (status == "PendingReturn")
            {
                lblHelper.Text = "STATUS: PENDING RETURN • Verify these items are returned:";
                lblHelper.ForeColor = Color.DarkMagenta;
            }
            else
            {
                lblHelper.Text = $"STATUS: {status.ToUpper()} • Items currently held:";
                lblHelper.ForeColor = Color.Gray;
            }

            // 3. Populate the ListBox
            var itemsList = itemsString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in itemsList)
            {
                lstItems.Items.Add($" •  {item}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}