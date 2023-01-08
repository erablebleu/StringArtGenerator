using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.Model;
using StringArtGenerator.App.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.App.Controls;
public static class PreviewBuilder
{
    public static Bitmap? Build(IList<ThreadAdapter> threads, IList<ThreadInstruction> instructions, int width, int height, bool lastRed = false, Brush? backColor = null)
    {
        if (threads is null || !threads.Any()
            || instructions is null || !instructions.Any()) return null;

        double scale = Math.Min(width / instructions.Max(n => n.Nail1RealPos.X),
                                height / instructions.Max(n => n.Nail1RealPos.Y));

        return Build(threads, instructions, scale, lastRed, backColor);
    }
    public static Bitmap? Build(IList<ThreadAdapter> threads, IList<ThreadInstruction> instructions, double scale, bool lastRed = false, Brush? backColor = null)
    {
        if (threads is null || !threads.Any()
            || instructions is null || !instructions.Any()) return null;

        double xMax = instructions.Max(n => n.Nail1RealPos.X);
        double yMax = instructions.Max(n => n.Nail1RealPos.Y);

        Bitmap bitmap = new((int)(xMax * scale), (int)(yMax * scale));
        List<Pen> pens = threads.Select(t => new Pen(new SolidBrush(Color.FromArgb((int)(255*t.PreviewThickness*scale), t.Color.ToDrawingColor())))).ToList();

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            Rectangle ImageSize = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            g.FillRectangle(backColor ?? Brushes.Transparent, ImageSize);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            foreach(ThreadInstruction instruction in instructions)
            { 
                Pen p = (instructions.Last() == instruction && lastRed)
                    ? new Pen(new LinearGradientBrush(new PointF((float)(instruction.Nail1RealPos.X * scale), (float)(instruction.Nail1RealPos.Y * scale)),
                                                      new PointF((float)(instruction.Nail2RealPos.X * scale), (float)(instruction.Nail2RealPos.Y * scale)),
                                                      pens[instruction.ThreadIndex].Color,
                                                      Color.Red))
                    : pens[instruction.ThreadIndex];

                g.DrawLine(p,
                    (float)(instruction.Nail1RealPos.X * scale),
                    (float)(instruction.Nail1RealPos.Y * scale),
                    (float)(instruction.Nail2RealPos.X * scale),
                    (float)(instruction.Nail2RealPos.Y * scale));
            }
        }

        bitmap.Save("preview.test.bmp");

        return bitmap;
    }
}
