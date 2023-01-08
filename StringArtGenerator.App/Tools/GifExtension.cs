using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.Controls;
using StringArtGenerator.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Tools;

public static class GifExtension
{
    public static void ExportGif(string filePath, IEnumerable<ThreadInstruction> instructions, int maxSize = 500, int stepPerFrame = 100, 
        int frameDuration = 50, int lastFrameDuration = 5000, double lineThickness = 0.1)
    {
        double xMin = instructions.Min(n => n.Nail2RealPos.X);
        double xMax = instructions.Max(n => n.Nail2RealPos.X);
        double yMin = instructions.Min(n => n.Nail2RealPos.Y);
        double yMax = instructions.Max(n => n.Nail2RealPos.Y);
        Canvas canvas = new()
        {
            Width = xMax - xMin,
            Height = yMax - yMin,
            Background = Brushes.White
        };
        Size size = new(xMax - xMin, yMax - yMin);
        canvas.Measure(size);
        canvas.Arrange(new Rect(size));
        RenderTargetBitmap renderBitmap = new((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Default);
        using GifWriter gifWrtier = new(filePath, frameDuration, 0);

        int idx = 0;
        while (idx < instructions.Count())
        {
            for (int i = idx; i < idx + stepPerFrame && i < instructions.Count(); i++)
            {
                canvas.Children.Add(new System.Windows.Shapes.Line()
                {
                    X1 = instructions.ElementAt(i).Nail1RealPos.X - xMin,
                    Y1 = instructions.ElementAt(i).Nail1RealPos.Y - yMin,
                    X2 = instructions.ElementAt(i).Nail2RealPos.X - xMin,
                    Y2 = instructions.ElementAt(i).Nail2RealPos.Y - yMin,
                    Stroke = Brushes.Black,
                    StrokeThickness = lineThickness,
                });
            }
            canvas.UpdateLayout();
            renderBitmap.Render(canvas);
            idx += stepPerFrame;
            gifWrtier.WriteFrame(renderBitmap.ToBitmap<GifBitmapEncoder>(), idx < instructions.Count() ? frameDuration : lastFrameDuration);
        }
    }
    public static void ExportGif2(string filePath, 
                                  IEnumerable<ThreadInstruction> instructions,
                                  List<ThreadAdapter> threads,
                                  int stepPerFrame = 100,
        int frameDuration = 50, int lastFrameDuration = 5000, double lineThickness = 0.1)
    {
        double xMin = instructions.Min(n => n.Nail2RealPos.X);
        double xMax = instructions.Max(n => n.Nail2RealPos.X);
        double yMin = instructions.Min(n => n.Nail2RealPos.Y);
        double yMax = instructions.Max(n => n.Nail2RealPos.Y);
        using GifWriter gifWrtier = new(filePath, frameDuration, 0);

        int idx = 0;
        while (idx < instructions.Count())
        {
            idx += stepPerFrame;
            System.Drawing.Bitmap bmp = PreviewBuilder.Build(threads, instructions.Take(Math.Min(instructions.Count(), idx)).ToList(), 0.5, backColor : System.Drawing.Brushes.White);
            int duration = idx < instructions.Count() ? frameDuration : lastFrameDuration;
            gifWrtier.WriteFrame(bmp, duration);
        }
    }
}