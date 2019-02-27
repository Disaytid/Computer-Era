using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Computer_Era.Game.Graphics
{
    public class GameGraphics
    {
        public BitmapImage GlueImages(BitmapImage cover, BitmapImage frame)
        {
            Bitmap baseImage = BitmapImage2Bitmap(cover);
            Bitmap overlayImage = BitmapImage2Bitmap(frame);

            var finalImage = new Bitmap(overlayImage.Width, overlayImage.Height);
            var graphics = System.Drawing.Graphics.FromImage(finalImage);
            graphics.CompositingMode = CompositingMode.SourceOver;

            graphics.DrawImage(baseImage, 0, 0);
            graphics.DrawImage(overlayImage, 0, 0);

            return Bitmap2BitmapImage(finalImage);
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            System.Windows.Media.Imaging.BitmapSource b =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                       bitmap.GetHbitmap(),
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());
            return (BitmapImage)b;
        }
    }
}
