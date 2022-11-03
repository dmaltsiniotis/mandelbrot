using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FractalMadness.CustomBitmap
{
    public class Format1bppIndexed : DirectBitmap
    {
        private readonly PixelFormat pixelFormat = PixelFormat.Format1bppIndexed;
        private readonly int paddingBytes = 0;
        private int WidthBytes { get; set; }
        private int Stride { get; set; }

        public Format1bppIndexed(int width, int height)
        {
            Width = width;
            Height = height;

            WidthBytes = Width / 8;
            WidthBytes += (Width % 8 != 0 ? 1 : 0); // If the width is not evenly divisible by 8, add one extra byte for the remaining bits of the width.
            paddingBytes = (WidthBytes % 4) != 0 ? 4 - (WidthBytes % 4) : 0;

            Stride = WidthBytes + paddingBytes;

            Bytes = new byte[Stride * Height];
            BytesHandle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Stride, pixelFormat, BytesHandle.AddrOfPinnedObject());
        }

        public override void SetPixel(int x, int y, Color color)
        {
            // Minor optimization: This format defaults to black because the byte array is initialized to zero. If we're only doing one pass, we can skip setting black since it's already black.
            //if (color == Color.Black)
            //    return;
            
            if (Bytes != null)
            {
                if (color == Color.Black || color == Color.White)
                {
                    int byteIndex = (x / 8) + (y * Stride);
                    int bitOffset = x % 8;
                    int bitOffsetMask = (int)Math.Pow(2, (7 - bitOffset));

                    if (color == Color.Black)
                    {
                        Bytes[byteIndex] = (byte)(Bytes[byteIndex] & ~bitOffsetMask);
                    }
                    else if (color == Color.White)
                    {
                        Bytes[byteIndex] = (byte)(Bytes[byteIndex] | bitOffsetMask);
                    }
                    else
                    {
                        throw new ArgumentException("Color can only be black or white when using the 1 bit per pixel format.", nameof(color));
                    }
                }
                else
                {
                    throw new ArgumentException("Color can only be black or white when using the 1 bit per pixel format.", nameof(color));
                }
            }
            else
            {
                throw new NullReferenceException("Bytes are not yet initilized. How did this happen?");
            }
        }

        // This needs to be tested, I'm not sure the logic is sound here. bitOffsetLeft may need a +1;
        public override Color GetPixel(int x, int y)
        {
            if (Bytes != null)
            {

                int byteIndex = (x / 8) + (y * Stride);
                int bitOffsetLeft = x % 8;
                int bitOffsetRight = 7 - bitOffsetLeft;
                
                byte byteToCheck = Bytes[byteIndex];
                byteToCheck = (byte)(byteToCheck >> bitOffsetRight);

                return (byteToCheck & 1) == 1 ? Color.White : Color.Black;
            }
            else
            {
                throw new NullReferenceException("Bytes are not yet initilized. How did this happen?");
            }
        }

        // This will only be called on a pixel that is in the set. Therefore we can always return "White", as in, "Not in the set" for _any_ value.
        public override Color InterpolateColor(int value, int max_value)
        {
            return Color.White;
        }
    }
}