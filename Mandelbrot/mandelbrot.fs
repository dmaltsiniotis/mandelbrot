open System.Numerics
open System.Drawing
open FSharp.Collections.ParallelSeq
open FractalMadness

// Tune-able parameters
let plotScalingFactor = 1000.0  // This is how large/detailed the x/y plot and resulting final image will be. The base plot/coordinate plane is sub-divided by this number to increase detail.
let max_iterations = 100        // How "deep" to check any particular point on the x/y plot to see if falls within the Mandelbrot set. More iterations = more/harder defined plot detail. Less iterations = softer color shading due to max iterations being hit earlier.

// This is the actual Mandelbrot set test, aka the "algorithm".
let mandelbrotTest c z = Complex.Pow(z, 2.0) + c 

let rec inSet c iterations z =
    let i = iterations - 1
    let (newz: Complex) = mandelbrotTest c z
    if newz.Magnitude > 2.0 then (c, false, i) else 
    if iterations > 0 then inSet c i newz else (c, true, i)

[<EntryPoint>]
let complex argv =
     let stopWatch = System.Diagnostics.Stopwatch.StartNew()
     let p0 = new Complex(0.0, 0.0)
     let stepSize = 1.0 / plotScalingFactor
     let x_plotsize = 1.5
     let x_plotshift = -0.5
     let y_plotsize = 1.2

     let plot =
      seq { for x in ((x_plotsize * -1.0) + x_plotshift) .. stepSize .. (x_plotsize + x_plotshift) do
             for y in y_plotsize * -1.0 .. stepSize .. y_plotsize do
              yield new Complex(x, y)}

     // This is the actual final graph/plot of the Mandelbrot set.
     let resultsToPlot = PSeq.map (fun x -> inSet x max_iterations p0) plot


     // ******************************************************************************************************************
     // * Everything below is related to rendering the plot as a bitmap. The core "Mandelbrot" code is everything above. * 
     // ******************************************************************************************************************
     let x_imagePixelSize = (int)(x_plotsize * 2.0 * plotScalingFactor) + 1
     let y_imagePixelSize = (int)(y_plotsize * 2.0 * plotScalingFactor) + 1

     //let imageResult = new CustomBitmap.DefaultBitmap(x_imagePixelSize, y_imagePixelSize)
     //let imageResult = new CustomBitmap.Format32bppArgb(x_imagePixelSize, y_imagePixelSize)
     let imageResult = new CustomBitmap.Format8bppIndexed(x_imagePixelSize, y_imagePixelSize)
     //let imageResult = new CustomBitmap.Format1bppIndexed(x_imagePixelSize, y_imagePixelSize)
     System.Console.WriteLine("Rendering a {0}x{1} ({2:n} pixels) at {3} bytes per pixel for a total of {4:n} megabytes of bitmap memory.", x_imagePixelSize, y_imagePixelSize, x_imagePixelSize * y_imagePixelSize, imageResult.BytesPerPixel(), ((float)x_imagePixelSize * (float)y_imagePixelSize * imageResult.BytesPerPixel()) / (float)(1000*1000))
     Seq.iter (fun (a : Complex, b, i) -> imageResult.SetPixel(System.Convert.ToInt32(((a.Real + (x_plotsize + abs(x_plotshift))) * plotScalingFactor)), System.Convert.ToInt32(((a.Imaginary + y_plotsize) * plotScalingFactor)), if b then Color.Black else imageResult.InterpolateColor(i, max_iterations))) resultsToPlot

     stopWatch.Stop()
     System.Console.WriteLine("Rending complete in {0:n} milliseconds ({1:n}s or {2:n}m). Saving bitmap to disk....", stopWatch.Elapsed.TotalMilliseconds, stopWatch.Elapsed.TotalSeconds, stopWatch.Elapsed.TotalMinutes)
     
     let renderFilename = System.String.Format("mandelbrot_{0}x{1}_{2}_{3}_{4}.png", x_imagePixelSize, y_imagePixelSize, plotScalingFactor, max_iterations, imageResult.PixelFormat)
     imageResult.Save(renderFilename, Imaging.ImageFormat.Png)
     
     System.Console.WriteLine("Done")
     
     #if DEBUG
     let procStartInfo = System.Diagnostics.ProcessStartInfo(FileName = "cmd.exe", Arguments = "/c start " + renderFilename)
     let p = new System.Diagnostics.Process(StartInfo = procStartInfo)
     p.Start() |> ignore
     #endif

     0 // return 0 from int main.