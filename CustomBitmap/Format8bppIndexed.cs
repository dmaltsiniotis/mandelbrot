using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FractalMadness.CustomBitmap
{
    public class Format8bppIndexed : DirectBitmap
    {
        private readonly PixelFormat pixelFormat = PixelFormat.Format8bppIndexed;
        private readonly int bytesPerPixel = 1;
        private readonly int padding = 0;

        public Format8bppIndexed(int width, int height)
        {
            Width = width;
            Height = height;

            padding = (Width % 4) != 0 ? 4 - (Width % 4) : 0;
            Bytes = new byte[(Width + padding) * Height * bytesPerPixel];
            BytesHandle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, (Width + padding) * bytesPerPixel, pixelFormat, BytesHandle.AddrOfPinnedObject());
            CreatePalette(Bitmap);
        }

        public override void SetPixel(int x, int y, Color color)
        {
            byte colorByte = BitConverter.GetBytes(color.ToArgb())[1];

            if (Bytes != null)
            {
                int startIndex = x * bytesPerPixel + (y * (Width + padding) * bytesPerPixel);
                Bytes[startIndex + 0] = colorByte;
            }
        }

        public override Color GetPixel(int x, int y)
        {
            if (Bitmap != null && Bytes != null)
            {
                int byteIndex = x * bytesPerPixel + (y * (Width + padding) * bytesPerPixel);
                return Bitmap.Palette.Entries[Bytes[byteIndex]];
            }
            else
            {
                throw new NullReferenceException("Can't " + nameof(GetPixel) + " because the underlying byte array is null and/or not initilized yet");
            }
        }

        // This should really be called InterpolateIndex because we're not selecting a color here, we're selecting an index of the color _palette_.
        // However, in an effort to keep the method/interface of SetPixel the same across all implementations, we using a 'Color' as carrier of information.
        // In this case, the 2nd byte/channel/color of the Color struct represents a 0-255 value that is used for the color palette index.
        // It _just so happens_ the color palette is indexed 256 grey-scale colors...
        public override Color InterpolateColor(int value, int max_value)
        {
            int interprolatedValue = InterpolateValue(value, max_value);
            return Color.FromArgb(byte.MaxValue, interprolatedValue, interprolatedValue, interprolatedValue);
        }

        private static void CreatePalette(Bitmap bitmap, RGBn monochromeColor = RGBn.None)
        {
            if (bitmap != null)
            {
                ColorPalette originalPallet = bitmap.Palette;
                if (originalPallet.Entries.Length != 256)
                {
                    throw new ArgumentOutOfRangeException(nameof(bitmap), "Bitmap color palette not in expected 8 bit (256) color format. Is this a Format8bppIndexed bitmap?");
                }
                for (int i = 0; i < originalPallet.Entries.Length; i++)
                {
                    originalPallet.Entries[i] = monochromeColor switch
                    {
                        RGBn.Red => Color.FromArgb(i, 0, 0),
                        RGBn.Green => Color.FromArgb(0, i, 0),
                        RGBn.Blue => Color.FromArgb(0, 0, i),
                        _ => Color.FromArgb(i, i, i),
                    };
                }
                bitmap.Palette = originalPallet;
            }
        }
    }
}