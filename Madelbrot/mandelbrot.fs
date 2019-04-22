open System.Numerics
open System.Drawing
open FSharp.Collections.ParallelSeq

let max_iterations = 100 // This controls the level of detail
let max_rgb = 255
let mandelbrotTest c z = Complex.Pow(z, 2.0) + c // This is the actual Mandelbrot set "algorithm"

let rec inSet c iterations z =
    let i = iterations - 1
    let newz = mandelbrotTest c z
    if newz.Magnitude > 2.0 then (c, false, i) else 
    if iterations > 0 then inSet c i newz else (c, true, i)

let colorMod iteration =
    let iteration = if iteration < 0 then 0 else iteration
    let interprolatedColorValue = (iteration * max_rgb) / max_iterations
    Color.FromArgb(interprolatedColorValue, interprolatedColorValue, interprolatedColorValue) // Maybe we can enhance this to return preetty colors in the future, but grey-scale looks good for now.

[<EntryPoint>]
let complex argv =
     let startTime = System.DateTime.Now
     let p0 = new Complex(0.0, 0.0)
     let plotScalingFactor = 100.0 // This essentially controls how large/detailed the final bitmap will be. I can only get this up to about 5000 on a 64bit windows system.
     let stepSize = 1.0 / plotScalingFactor
     let plotsize = 2.0 // This should always be 2.0 because when you plot the Mandelbrot set on a coordinate plane, it's range on the x-axis is -1 to 2.0.

     let plot =
      seq { for i in plotsize * -1.0 .. stepSize .. plotsize do
             for j in plotsize * -1.0 .. stepSize .. plotsize do
              yield new Complex(i, j)}

     let resultsToPlot = PSeq.map (fun x -> inSet x max_iterations p0) plot
     let imagePixelSize = (int)(plotsize * 2.0 * plotScalingFactor) + 1
     let imageResult = new Bitmap(imagePixelSize, imagePixelSize)
     
     Seq.iter (fun (a : Complex, b, i) -> imageResult.SetPixel(System.Convert.ToInt32(((a.Real + plotsize) * plotScalingFactor)), System.Convert.ToInt32(((a.Imaginary + plotsize) * plotScalingFactor)), if b then Color.Black else colorMod i)) resultsToPlot
     
     imageResult.Save("mandelbrot.bmp")
     System.Console.WriteLine("Done in {0} seconds.", (System.DateTime.Now - startTime).TotalSeconds.ToString())
     let procStartInfo = System.Diagnostics.ProcessStartInfo(FileName = "cmd.exe", Arguments = "/c start mandelbrot.bmp")
     let p = new System.Diagnostics.Process(StartInfo = procStartInfo)
     p.Start() |> ignore
     0