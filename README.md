# A Mandelbrot fractal generator written in F\#
(and supported by a C\# Bitmap helper class)

## Tips

To adjust the size of the final size of the bitmap, try varying the "plotScalingFactor" variable between 100, 500, 1000, and if you're feeling ambitious, 5000 or 10000.

To adjust the level of detail, increase the max_interations. This will have a dramatic impact on CPU consumption, as more recursive number-crunching needs to happen on each pixel.

A 10,000 plotScalingFactor (with a 100 max_iterations value) yields a 30001x24001 pixel image in about 5-6 minutes rendering time on a 18 core (36 logical core) CPU.

Implemented are multiple color formats via the new CustomBitmap C# project. Specifically:
* 32bppArgb - The default format for `new Bitmap()`
* 8bppIndexed - A 256 color version, currently implemented as a grey-scale palette.
* 1bppIndexed - A 1 color version, black or white.

To switch between them, use their corresponding class name by un-commentating the line. The best "bang for your buck" I've identified during testing is Format8bppIndexed for grey-scale images.

```
	//let imageResult = new CustomBitmap.DefaultBitmap(x_imagePixelSize, y_imagePixelSize)
	//let imageResult = new CustomBitmap.Format32bppArgb(x_imagePixelSize, y_imagePixelSize)
	//let imageResult = new CustomBitmap.Format8bppIndexed(x_imagePixelSize, y_imagePixelSize)
	//let imageResult = new CustomBitmap.Format1bppIndexed(x_imagePixelSize, y_imagePixelSize)
```

Your mileage may vary though, as the Bitmap class is a thin wrapper over GDI+ and shouldn't be used here in the first place.

Note: Due to 2BG 32-bit process and/or memory limitations, rendering anything over a plotScalingFactor of 8000 using the Bmp file format doesn't work. While the actual pixels are calculated and rendered in Bitmap format, for the sake of simplicity, the output is saved as Png which includes some compression. Otherwise you're looking at file sizes of _many_ gigabytes at extreme sizes.

## TODO:

In order to go to even higher resolutions (such as beyond 32000x32000), the application needs to be converted from int to Int64 everywhere, and target x64 architecture, and thoroughly fixed and re-tested.

Ideally, a non-GDI+ cross-platform library can replace System.Drawing.Bitmap for processing and outputting bitmaps. An option I'm considering is to directly craft the raw Bitmap file format such as this example: https://www.codeproject.com/Articles/70442/C-RGB-to-Palette-Based-8-bit-Greyscale-Bitmap-Clas

## Example outputs:

These renders were run in debug mode on an Intel Core i9 7980 XE (18 real cores, 36 hyper-threaded cores) with 64 GB RAM. This whole project was an exercise in discovering a way to utilize my idle CPU time :(

[plotScalingFactor 100: Low resolution ~1-2 seconds render time](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_lowres.bmp)

[plotScalingFactor 1000: Medium resolution ~14-15 seconds render time](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_medres.bmp)

[plotScalingFactor 5000: High resolution ~6-7 minutes render time](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_highres.bmp)

[plotScalingFactor 10000: Super High resolution 5.33 minutes render time](https://raw.githubusercontent.com/dmaltsiniotis/mandelbrot/master/Renders/mandelbrot_superhighres.png)

