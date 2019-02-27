
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Computer_Era.Game.Graphics
{
    public class GameGraphics
    {
        public BitmapImage GlueImages(BitmapImage cover, BitmapImage frame)
        {
            WriteableBitmap wb = new WriteableBitmap(frame);

            Chubs_BitBltMerge(ref wb, 0, 0, ref cover);
            return ConvertWriteableBitmapToBitmapImage(wb);
        }

        public void Chubs_BitBltMerge(ref WriteableBitmap dest, int nXDest, int nYDest, ref BitmapImage src)
        {
            int src_stride = src.PixelWidth * (src.Format.BitsPerPixel >> 3);
            byte[] src_buffer = new byte[src_stride * src.PixelHeight];
            src.CopyPixels(src_buffer, src_stride, 0);

            int dest_stride = src.PixelWidth * (dest.Format.BitsPerPixel >> 3);
            byte[] dest_buffer = new byte[(src.PixelWidth * src.PixelHeight) << 2];
            dest.CopyPixels(new Int32Rect(nXDest, nYDest, src.PixelWidth, src.PixelHeight), dest_buffer, dest_stride, 0);

            for (int i = 0; i < src_buffer.Length; i = i + 4)
            {
                float src_alpha = ((float)src_buffer[i + 3] / 255);
                dest_buffer[i + 0] = (byte)((src_buffer[i + 0] * src_alpha) + dest_buffer[i + 0] * (1.0 - src_alpha));
                dest_buffer[i + 1] = (byte)((src_buffer[i + 1] * src_alpha) + dest_buffer[i + 1] * (1.0 - src_alpha));
                dest_buffer[i + 2] = (byte)((src_buffer[i + 2] * src_alpha) + dest_buffer[i + 2] * (1.0 - src_alpha));
            }

            dest.WritePixels(new Int32Rect(nXDest, nYDest, src.PixelWidth, src.PixelHeight), dest_buffer, dest_stride, 0);
        }

        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }
    }
}
