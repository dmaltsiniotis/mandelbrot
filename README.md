# A Mandelbrot fractal generator written in F\#

## Tips

To adjust the size of the final size of the bitmap, try varying the "plotScalingFactor" variable between 100, 500, 1000, and if you're feeling ambitious, 5000. I personally have not been able to create a bitmap on 64bit windows with 64gb of ram greater than 5000x5000 using System.Drawing.Bitmap. Your mileage may vary though as the Bitmap class is a thin wrapper over GDI+ and shouldn't be used here in the first place.

## Example outputs:

These renders were run in debug mode on an Intel Core i9 7980 XE (18 real cores, 36 hyper-threaded cores) with 64 GB RAM. This whole project was an exercise in discovering a way to utilize my idle CPU time :(

plotScalingFactor 100: [Low res ~1-2 seconds](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_lowres.bmp)
plotScalingFactor 1000: [Medium resolution ~14-15 seconds](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_medres.bmp)
plotScalingFactor 5000: [High Resolution ~6-7 minutes](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_highres.bmp)

