using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace Ventrix.Application.Services
{
    public class QrCodeService
    {
        /// <summary>
        /// Generates a QR Code for a user with their name clearly printed below it.
        /// </summary>
        public Bitmap GenerateUserQrCodeWithLabel(string userId, string userName)
        {
            string payload = $"USER:{userId}";
            string displayText = $"{userName}\nID: {userId}";
            return CreateCompositeQrImage(payload, displayText);
        }

        /// <summary>
        /// Generates a QR Code for an item with its specific unit name printed below it.
        /// </summary>
        public Bitmap GenerateItemQrCodeWithLabel(int itemId, string itemName)
        {
            string payload = $"ITEM:{itemId}";
            string displayText = itemName;
            return CreateCompositeQrImage(payload, displayText);
        }

        private Bitmap CreateCompositeQrImage(string payload, string labelText)
        {
            // 1. Generate the base QR code image
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(10, Color.Black, Color.White, true))
                    {
                        // 2. Define dimensions for the final "Sticker"
                        int textSpace = 70; // Extra pixels at the bottom to fit the text
                        int width = qrBitmap.Width;
                        int height = qrBitmap.Height + textSpace;

                        // 3. Create the blank canvas
                        Bitmap finalImage = new Bitmap(width, height);

                        // 4. Paint the QR code and text onto the canvas
                        using (Graphics graphics = Graphics.FromImage(finalImage))
                        {
                            // Ensure the text rendering is smooth
                            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                            // Fill background with solid white
                            graphics.Clear(Color.White);

                            // Draw the QR Code at the very top (X: 0, Y: 0)
                            graphics.DrawImage(qrBitmap, 0, 0);

                            // Setup the font and center-alignment for the text
                            using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
                            using (StringFormat format = new StringFormat())
                            {
                                format.Alignment = StringAlignment.Center;
                                format.LineAlignment = StringAlignment.Near;

                                // Define the exact rectangle area where the text belongs
                                RectangleF textRect = new RectangleF(0, qrBitmap.Height, width, textSpace);

                                // Draw the human-readable text
                                graphics.DrawString(labelText, font, Brushes.Black, textRect, format);
                            }
                        }

                        return finalImage;
                    }
                }
            }
        }

        /// <summary>
        /// Saves the generated QR sticker to a designated folder on the computer.
        /// </summary>
        public void SaveQrSticker(Bitmap qrImage, string folderPath, string fileName)
        {
            // Create the directory if it doesn't exist yet
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Clean the filename of any invalid characters (like slashes in names)
            string safeFileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
            string fullPath = Path.Combine(folderPath, $"{safeFileName}.png");

            qrImage.Save(fullPath, ImageFormat.Png);
        }
    }
}