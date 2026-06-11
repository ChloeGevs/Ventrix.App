using System;
using System.Text;
using System.Windows.Forms;

namespace Ventrix.Application.Services
{
    public class QrScannerService
    {
        private StringBuilder _scanBuffer = new StringBuilder();

        // Explicitly define the Windows Forms Timer to fix the CS0104 error
        private System.Windows.Forms.Timer _keystrokeTimer;

        // This event fires the moment a complete QR code is processed
        public event Action<string>? OnBarcodeScanned;

        public QrScannerService()
        {
            // Explicitly instantiate the Windows Forms Timer
            _keystrokeTimer = new System.Windows.Forms.Timer();

            // 50 milliseconds is the sweet spot. 
            // Humans can't type 2 keys in 50ms, but a scanner easily can.
            _keystrokeTimer.Interval = 50;
            _keystrokeTimer.Tick += KeystrokeTimer_Tick;
        }

        /// <summary>
        /// Call this method from your Form's KeyPress event.
        /// </summary>
        public void ProcessKeystroke(KeyPressEventArgs e)
        {
            // Reset the timer every time a new key is pressed
            _keystrokeTimer.Stop();

            // If the scanner explicitly sends an "Enter" key at the end of the scan
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return)
            {
                ProcessCompletedScan();
                e.Handled = true; // Prevent the default "ding" sound
                return;
            }

            // Append the character to our buffer and restart the countdown
            _scanBuffer.Append(e.KeyChar);
            _keystrokeTimer.Start();
        }

        private void KeystrokeTimer_Tick(object? sender, EventArgs e)
        {
            // If 50ms passes without a keystroke, we assume the scan (or typing) is done
            _keystrokeTimer.Stop();
            ProcessCompletedScan();
        }

        private void ProcessCompletedScan()
        {
            if (_scanBuffer.Length > 0)
            {
                string scannedData = _scanBuffer.ToString().Trim();
                _scanBuffer.Clear();

                // Only trigger the event if it's a reasonably long string 
                // (e.g., ignoring single accidental keystrokes)
                if (scannedData.Length >= 3)
                {
                    OnBarcodeScanned?.Invoke(scannedData);
                }
            }
        }
    }
}