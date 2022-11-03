using Microsoft.VisualBasic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FractalMadness.CustomBitmap
{
    // The goal should be to dump the GDI+ wrapped, Windows-only, System.Drawing.Bitmap completely in favor of something faster and cross-platform.
    // Until then, this little enhancement greatly improves render performance by at least 3-4x, from 6-8 minutes to 2 minutes at a 1000 plotsize value.
    // https://stackoverflow.com/questions/24701703/c-sharp-faster-alternatives-to-setpixel-and-getpixel-for-bitmaps-for-windows-f
    // Alternative Bitmap Set/Get pixel code by https://stackoverflow.com/users/3117338/a-konzel
    public abstract class DirectBitmap : IDisposable
    {
        public Bitmap? Bitmap { get; protected set; }
        public byte[]? Bytes { get; protected set; }
        public bool Disposed { get; protected set; }
        public int Height { get; protected set; }
        public int Width { get; protected set; }
        protected GCHandle BytesHandle { get; set; }

        public PixelFormat PixelFormat
        {
            get
            {
                return Bitmap != null ? Bitmap.PixelFormat : PixelFormat.Undefined;
            }
        }

        public abstract void SetPixel(int x, int y, Color color);

        public abstract Color GetPixel(int x, int y);

        public abstract Color InterpolateColor(int value, int max_value);

        public void Save(string filename, ImageFormat imageFormat)
        {
            if (Bitmap != null)
            {
                Bitmap.Save(filename, imageFormat);
            }
            else
            {
                throw new InvalidOperationException("Unable to save, Bitmap not initialized.");
            }
        }

        protected static byte InterpolateValue(int value, int max_value)
        {
            value = value < 0 ? 0 : value;
            return (byte)((value * byte.MaxValue) / max_value);
        }

        public double BytesPerPixel()
        {
            if (Bitmap != null)
            {
                var bpp = Bitmap.PixelFormat switch
                {
                    PixelFormat.Format1bppIndexed => 1.0f / 8.0f,
                    PixelFormat.Format4bppIndexed => 1,
                    PixelFormat.Format8bppIndexed => 1,
                    PixelFormat.Format16bppArgb1555 or PixelFormat.Format16bppGrayScale or PixelFormat.Format16bppRgb555 or PixelFormat.Format16bppRgb565 => 2,
                    PixelFormat.Format24bppRgb => 3,
                    PixelFormat.Format32bppArgb or PixelFormat.Format32bppPArgb or PixelFormat.Format32bppRgb => 4,
                    PixelFormat.Format48bppRgb => 6,
                    PixelFormat.Format64bppArgb or PixelFormat.Format64bppPArgb => 8,
                    _ => throw new InvalidOperationException("Unknown format: " + Bitmap.PixelFormat.ToString() + " to infer bytes per pixel."),
                };
                return bpp;
            }
            else
            {
                throw new InvalidOperationException("Internal Bitmap is not initialized. How did this happen?");
            }
        }

        ~DirectBitmap()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            if (Bitmap != null) Bitmap.Dispose();
            BytesHandle.Free();
            GC.SuppressFinalize(this);
        }
    }

    public enum RGBn
    {
        Red,
        Green,
        Blue,
        None
    }
}