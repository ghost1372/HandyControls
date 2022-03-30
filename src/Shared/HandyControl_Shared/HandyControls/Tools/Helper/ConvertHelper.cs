using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace HandyControl.Tools;

public static class ConvertHelper
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

    /// <summary>
    /// Translate numeric file size in bytes to a human-readable shorter string format.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string ToFileSize(long size)
    {
        if (size < 1024)
        {
            return size.ToString("F0") + " bytes";
        }
        else if ((size >> 10) < 1024)
        {
            return (size / 1024F).ToString("F1") + " KB";
        }
        else if ((size >> 20) < 1024)
        {
            return ((size >> 10) / 1024F).ToString("F1") + " MB";
        }
        else if ((size >> 30) < 1024)
        {
            return ((size >> 20) / 1024F).ToString("F1") + " GB";
        }
        else if ((size >> 40) < 1024)
        {
            return ((size >> 30) / 1024F).ToString("F1") + " TB";
        }
        else if ((size >> 50) < 1024)
        {
            return ((size >> 40) / 1024F).ToString("F1") + " PB";
        }
        else
        {
            return ((size >> 50) / 1024F).ToString("F0") + " EB";
        }
    }
}
