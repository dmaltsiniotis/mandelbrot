using System.Drawing;

namespace FractalMadness.CustomBitmap
{
    public class DefaultBitmap : DirectBitmap
    {
        public DefaultBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bitmap = new Bitmap(width, height);
        }

        public override void SetPixel(int x, int y, Color color)
        {
            if (Bitmap != null)
            {
                Bitmap.SetPixel(x, y, color);
            }
        }

        public override Color GetPixel(int x, int y)
        {
            if (Bitmap != null)
            {
                return Bitmap.GetPixel(x, y);
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