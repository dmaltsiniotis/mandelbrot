using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FractalMadness.CustomBitmap
{
    public class Format32bppArgb : DirectBitmap
    {
        private readonly PixelFormat pixelFormat = PixelFormat.Format32bppArgb;
        private readonly int bytesPerPixel = 4;

        public Format32bppArgb(int width, int height)
        {
            Width = width;
            Height = height;
            Bytes = new byte[width * height * bytesPerPixel];
            BytesHandle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * bytesPerPixel, pixelFormat, BytesHandle.AddrOfPinnedObject());
        }

        public override void SetPixel(int x, int y, Color color)
        {
            byte[] colorBytes = BitConverter.GetBytes(color.ToArgb());

            if (Bytes != null)
            {
                int startIndex = x * bytesPerPixel + (y * Width * bytesPerPixel);

                Bytes[startIndex + 0] = colorBytes[0];
                Bytes[startIndex + 1] = colorBytes[1];
                Bytes[startIndex + 2] = colorBytes[2];
                Bytes[startIndex + 3] = colorBytes[3];
            }
        }

        public override Color GetPixel(int x, int y)
        {
            if (Bytes != null)
            {
                int startIndex = x * bytesPerPixel + (y * Width * bytesPerPixel);
                return Color.FromArgb(Bytes[startIndex + 0], Bytes[startIndex + 1], Bytes[startIndex + 2], Bytes[startIndex + 3]);
            }
            else
            {
                throw new NullReferenceException("Can't " + nameof(GetPixel) + " because the underlying byte array is null and/or not initilized yet");
            }
        }

        public override Color InterpolateColor(int value, int max_value)
        {
            int interprolatedValue = InterpolateValue(value, max_value);
            return Color.FromArgb(byte.MaxValue, interprolatedValue, interprolatedValue, interprolatedValue);
        }
    }
}