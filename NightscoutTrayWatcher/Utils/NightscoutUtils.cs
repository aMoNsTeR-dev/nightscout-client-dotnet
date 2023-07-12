using NightscoutClientDotNet.Models;
using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NightscoutTrayWatcher.Utils
{
    internal static class NightscoutUtils
    {
        // https://stackoverflow.com/a/58307732
        public static string GetBasePath()
        {
            using var processModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            return System.IO.Path.GetDirectoryName(processModule?.FileName);
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static System.Windows.Media.ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle);
            }
        }

        public static Icon GetIconFromEntry(Entry entry)
        {
            System.Windows.Media.ImageSource imageSource = GetImageSourceFromEntry(entry);

            if (imageSource == null) return null;

            if (imageSource.ToString().Equals("System.Windows.Interop.InteropBitmap"))
            {
                Bitmap bmp2;
                Icon newIcon;
                BitmapSource srs = (BitmapSource)imageSource;
                int width = srs.PixelWidth;
                int height = srs.PixelHeight;
                int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
                IntPtr ptr = IntPtr.Zero;
                IntPtr hIcon = IntPtr.Zero;
                try
                {
                    ptr = Marshal.AllocHGlobal(height * stride);
                    srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
                    using (var btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr))
                    {
                        // Clone the bitmap so that we can dispose it and
                        // release the unmanaged memory at ptr
                        bmp2 = new System.Drawing.Bitmap(btm);
                    }

                    //hIcon = bmp2.GetHicon();
                    //newIcon = Icon.FromHandle(hIcon);
                    newIcon = BitmapToIcon(bmp2);
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptr);

                    if (hIcon != IntPtr.Zero)
                        DeleteObject(hIcon);
                }

                if (newIcon != null)
                {
                    return newIcon;
                }
            }

            return null;
        }

        private static System.Windows.Media.ImageSource GetImageSourceFromEntry(Entry entry)
        {
            string text = ((int)entry.Sgv).ToString(CultureInfo.InvariantCulture);
            string arrow = entry.Arrow;

            System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
            if ((System.DateTime.Now - entry.DateTime).Minutes > 10)
            {
                fontStyle = System.Drawing.FontStyle.Strikeout;
            }

            Font fontToUse = new Font("Tahoma", 10, fontStyle, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.White);
            Bitmap bitmapText = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = System.Drawing.Graphics.FromImage(bitmapText);
            g.Clear(Color.Transparent);

            //g.FillEllipse(Brushes.Gray, 0, 0, 18, 18);
            //g.FillEllipse(Brushes.Black, 0, 0, 16, 16);

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.DrawString(text, fontToUse, brushToUse, -2, -2);

            Font fontToUseArrow = new Font("Arial", 11, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(arrow, fontToUseArrow, brushToUse, 2, 5);

            return ImageSourceForBitmap(bitmapText);
        }

        /// <summary>
        /// Create a Icon that calls DestroyIcon() when the Destructor is called.
        /// Unfortunatly Icon.FromHandle() initializes with the internal Icon-constructor Icon(handle, false), which sets the internal value "ownHandle" to false
        /// This way because of the false, DestroyIcon() is not called as can be seen here:
        /// https://referencesource.microsoft.com/#System.Drawing/commonui/System/Drawing/Icon.cs,f2697049dea34e7c,references
        /// To get arround this we get the constructor internal Icon(IntPtr handle, bool takeOwnership) from Icon through reflection and initialize that way
        /// </summary>
        ///
        /// https://gist.github.com/D4koon/2b397eca452b75115f19063560efbbb3
        /// https://stackoverflow.com/a/1551567/11317043
        private static Icon BitmapToIcon(Bitmap bitmap)
        {
            Type[] cargt = new[] { typeof(IntPtr), typeof(bool) };
            ConstructorInfo ci = typeof(Icon).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, cargt, null);
            object[] cargs = new[] { (object)bitmap.GetHicon(), true };
            Icon icon = (Icon)ci.Invoke(cargs);
            return icon;
        }
    }
}
