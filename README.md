# A Mandelbrot fractal generator written in F\#

![Mandlebrot Set](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_lowres.bmp)

## Info

This project currently supports .NET Core 3.1. However, there is an annoying dependency on System.Drawing in order to output the final bitmap. Running the console application will generate file called mandelbrot.bmp in the current run directory.

## Tips

To adjust the size of the final size of the bitmap, try varying the "plotScalingFactor" variable between 100, 500, 1000, and if you're feeling ambitious, 5000. I personally have not been able to create a bitmap on 64bit windows with 64gb of ram greater than 5000x5000 using System.Drawing.Bitmap. Your mileage may vary though as the Bitmap class is a thin wrapper over GDI+ and shouldn't be used here in the first place.

## Examples

These renders were run in debug mode on an Intel Core i9 7980 XE (18 real cores, 36 hyper-threaded cores) with 64 GB RAM. This whole project was an exercise in discovering a way to utilize my idle CPU time :(

[plotScalingFactor 100: Low res ~1-2 seconds render time](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_lowres.bmp)

[plotScalingFactor 1000: Medium resolution ~14-15 seconds render time](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_medres.bmp)

[plotScalingFactor 5000: High Resolution ~6-7 minutes render time](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_highres.bmp)

## TODO

* It would be fantastic to change the "rendering engine" of System.Drawing.Bitmap to support something cross-platorm and efficient.
* Colorize the iterations to make the plot more pretty.
