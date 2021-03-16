using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace HandyControl.Tools
{
    public class ConvertHelper
    {
        public static BitmapImage BytesToBitmapImage(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static byte[] BitmapImageToBytes(BitmapImage bitmapImage)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        public static string BitmapImageToBase64(BitmapImage bitmapImage)
        {
            return Convert.ToBase64String(BitmapImageToBytes(bitmapImage));
        }

        public static BitmapImage Base64ToBitmapImage(string base64)
        {
            byte[] binaryData = Convert.FromBase64String(base64);
            return BytesToBitmapImage(binaryData);
        }
    }
}
