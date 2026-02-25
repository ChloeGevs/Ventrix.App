using System;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class InitializingApp : Form
    {
        private bool isFadingOut = false;

        public InitializingApp()
        {
            InitializeComponent();
            this.Opacity = 1; // Start at full visibility
            fadeTimer.Interval = 10; // Keep the interval for smooth transitions
        }

        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            if (isFadingOut)
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.02; // Slowly decrease visibility
                }
                else
                {
                    fadeTimer.Stop();
                    this.Close(); // Close the form once invisible
                }
            }
        }

        // Method to trigger the fade out from Program.cs
        public void StartFadeOut()
        {
            isFadingOut = true;
        }
    }
}