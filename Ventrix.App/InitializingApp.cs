using System;
using System.Windows.Forms;
using Ventrix.Application.Services;

namespace Ventrix.App
{
    public partial class InitializingApp : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly UserService _userService;

        private bool isFadingOut = false;

        // Inside InitializingApp.cs constructor
        public InitializingApp(InventoryService invService, BorrowService borrowService, UserService userService)
        {
            InitializeComponent();
            _inventoryService = invService;
            _borrowService = borrowService;
            _userService = userService;

            this.Opacity = 1;
            fadeTimer.Interval = 10;
            fadeTimer.Start();

            // FIX: Use the full name System.Windows.Forms.Timer
            System.Windows.Forms.Timer loadingDelay = new System.Windows.Forms.Timer();
            loadingDelay.Interval = 5000;
            loadingDelay.Tick += (s, e) =>
            {
                loadingDelay.Stop();
                StartFadeOut();
            };
            loadingDelay.Start();
        }

        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            if (isFadingOut)
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.05;
                }
                else
                {
                    fadeTimer.Stop();
                    Hide();

                    StartApp();
                }
            }
        }

        private void StartApp()
        {
            // Use the services passed from Program.cs
            BorrowerPortal portal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
            portal.Show();

            // Use Hide instead of Close so the application doesn't terminate
            this.Hide();
        }

        public void StartFadeOut()
        {
            isFadingOut = true;
        }

    }
}