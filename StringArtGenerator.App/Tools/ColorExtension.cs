using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.App.Tools;
public static class ColorExtension
{
    public static (byte h, byte s, byte l) RGBToHSL(byte r, byte g, byte b)
    {
        double h, s, l;
        (h, s, l) = RGBToHSL(r / 255d, g / 255d, b / 255d);
        return ((byte)(255 * h), (byte)(255 * s), (byte)(255 * l));
    }
    public static (byte h, byte s, byte l) RGBToHSV(byte r, byte g, byte b)
    {
        double h, s, v;
        (h, s, v) = RGBToHSV(r / 255d, g / 255d, b / 255d);
        return ((byte)(255 * h), (byte)(255 * s), (byte)(255 * v));
    }
    public static (byte r, byte g, byte b) HSLToRGB(byte h, byte s, byte l)
    {
        double r, g, b;
        (r, g, b) = HSLToRGB(h / 255d, s / 255d, l / 255d);
        return ((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
    }
    public static (byte r, byte g, byte b) HSVToRGB(byte h, byte s, byte v)
    {
        double r, g, b;
        (r, g, b) = HSVToRGB(h / 255d, s / 255d, v / 255d);
        return ((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
    }

    /// <summary>
    /// Convert RGB color to HSL (Hue, Saturation, Luminosity)
    /// </summary>
    /// <param name="r">[0;1] red component</param>
    /// <param name="g">[0;1] green component</param>
    /// <param name="b">[0;1] blue component</param>
    /// <returns>[0;1] HSL components</returns>
    public static (double h, double s, double l) RGBToHSL(double r, double g, double b)
    {
        double min = Math.Min(Math.Min(r, g), b);
        double max = Math.Max(Math.Max(r, g), b);
        double d = max - min;

        double h = 0;
        double l = (max + min) / 2.0d;

        if (d == 0)
            return (0, 0, l);

        double s = l < 0.5d 
            ? (double)(d / (max + min))
            : (double)(d / (2.0d - max - min));

        if (r == max)
            h = (g - b) / d / 6d;
        else if (g == max)
            h = 1d/3d + (b - r) / d / 6d;
        else
            h = 2d/3d + (r - g) / d / 6d;

        return (h, s, l);
    }

    public static (double r, double g, double b) HSLToRGB(double h, double s, double l)
    {
        static double GetHue(double p, double q, double t)
        {
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            else if (t < 1.0 / 2) return q;
            else if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }

        double q = (l < 0.5d)
            ? l * (1d + s)
            : l + s - l * s;
        double p = 2d * l - q;

        if (l == 0)
            return (0, 0, 0);
        if (s == 0)
            return (l, l, l);

        return (GetHue(p, q, h + 1d / 3d),
            GetHue(p, q, h),
            GetHue(p, q, h - 1d / 3d));
    }
 
    public static (double h, double s, double v) RGBToHSV(double r, double g, double b)
    {
        double min = Math.Min(Math.Min(r, g), b);
        double max = Math.Max(Math.Max(r, g), b);
        double d = max - min;

        double h = 0;
        double v = max;

        if (max == 0)
            return (0, 0, v);

        double s = d / max;

        if (r == max)
            h = (g - b) / d / 6d;
        else if (g == max)
            h = 1d / 3d + (b - r) / d / 6d;
        else
            h = 2d / 3d + (r - g) / d / 6d;

        return (h, s, v);
    }

    public static (double r, double g, double b) HSVToRGB(double h, double s, double v)
    {
        if (s == 0)
            return (v, v, v);

        h *= 6;            // sector 0 to 5
        int i = (int)Math.Floor(h);
        double f = h - i;          // factorial part of h
        double p = v * (1 - s);
        double q = v * (1 - s * f);
        double t = v * (1 - s * (1 - f));

        switch (i)
        {
            case 0: return (v, t, p);
            case 1: return (q, v, p);
            case 2: return (p, v, t);
            case 3: return (p, q, v);
            case 4: return (t, p, v);
            default: return (v, p, q);
        }
    }


    public static (byte h, byte s, byte l) GetHSL(this Color color) => RGBToHSL(color.R, color.G, color.B);
    public static (byte h, byte s, byte l) GetHSL(this System.Windows.Media.Color color) => RGBToHSL(color.R, color.G, color.B);
    public static (byte h, byte s, byte v) GetHSV(this Color color) => RGBToHSV(color.R, color.G, color.B);
    public static (byte h, byte s, byte v) GetHSV(this System.Windows.Media.Color color) => RGBToHSV(color.R, color.G, color.B);

    public static System.Windows.Media.Color ToMediaColor(this Color color)
        => System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

    public static Color ToDrawingColor(this System.Windows.Media.Color color)
        => Color.FromArgb(color.A, color.R, color.G, color.B);
    public static string ToHex(Color color)
        => $"#{color.A:x2}{color.R:x2}{color.G:x2}{color.B:x2}";
    public static Color FromHex(string hex)
        => ((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex)).ToDrawingColor();
}
