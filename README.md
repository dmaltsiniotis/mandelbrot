# A Benoit B. Mandelbrot generator written in F\#

## Tips

To adjust the size of the final size of the bitmap, try varying the "plotScalingFactor" variable between 100, 500, 1000, and if you're feeling ambitious, 5000. I personally have not been able to create a bitmap on 64bit windows with 64gb of ram greater than 5000x5000 using System.Drawing.Bitmap. Your mileage may vary though as the Bitmap class is a thin wrapper over GDI+ and shouldn't be used here in the first place.