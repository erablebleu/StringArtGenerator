using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.Imaging;

public enum Illuminant
{
    // https://en.wikipedia.org/wiki/Standard_illuminant#Illuminant_series_D
    D50, // Daylight, CCT 5003 K
    D65, // Daylight, CCT 6504 K
}

public record IlluminantData(double Xn, double Yn, double Zn);



public class ExtendedColor
{
    public static Dictionary<Illuminant, IlluminantData> Illuminants = new()
    {
        { Illuminant.D50, new IlluminantData(96.4212, 100, 82.5188) },
        { Illuminant.D65, new IlluminantData(95.0489, 100, 108.8840) },
    };

    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }
    public double L { get; set; }
    public double a { get; set; }
    public double b { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Color Color { get; set; }
    public ExtendedColor() { }
    public ExtendedColor(Color color) {
        Color = color;
        R = color.R;
        G = color.G;
        B = color.B;
        (X, Y, Z) = GetCIEXYZ(R/255, G/255, B/255);
        (L, a, b) = GetCIELAB(X, Y, Z);
    }

    public static (double X, double Y, double Z) GetCIEXYZ(double r, double g, double b)
    {
        double Pivot(double n) => (n > 0.04045 ? Math.Pow((n + 0.055) / 1.055, 2.4) : n / 12.92) * 100;

        r = Pivot(r);
        g = Pivot(g);
        b = Pivot(b);

        // (R,G,B)=M(X,Y,Z)
        // https://en.wikipedia.org/wiki/CIE_1931_color_space
        return (r * 0.4124 + g * 0.3576 + b * 0.1805,
                r * 0.2126 + g * 0.7152 + b * 0.0722,
                r * 0.0193 + g * 0.1192 + b * 0.9505);
    }

    public static (double R, double G, double B) GetRGB(double x, double y, double z)
    {
        // (R,G,B)=M^-1(X,Y,Z)
        return (x * 2.36461385 + y * -0.89654057 + z * -0.46807328,
            x * -0.51516621 + y * 1.4264081 + z * 0.0887581,
            x * 0.0052037 + y * -0.01440816 + z * 1.00920446);
    }

    public static (double L, double a, double b) GetCIELAB(double x, double y, double z, Illuminant illuminant = Illuminant.D65)
    {
        // https://en.wikipedia.org/wiki/CIELAB_color_space
        // https://github.com/hvalidi/ColorMine/blob/master/ColorMine/ColorSpaces/Conversions/LabConverter.cs
        IlluminantData id = Illuminants[illuminant];

        double d = 6 / 29d;
        double f(double t) => t > Math.Pow(d, 3) 
            ? Math.Pow(t, 1 / 3d)
            : t / (3 * Math.Pow(d, 2)) + 4 / 29d;

        return (116 * f(y / id.Yn) - 16,
            500 * (f(x / id.Xn) - f(y / id.Yn)),
            200 * (f(y / id.Yn) - f(z / id.Zn)));
    }

    public double Compare(ExtendedColor c)
    {
        return Math.Sqrt(Math.Pow(L - c.L, 2) + Math.Pow(a - c.a, 2) + Math.Pow(b - c.b, 2));
    }
    public static double DeltaE(ExtendedColor c0, ExtendedColor c1, DeltaEMode mode = DeltaEMode.CIE76)
    {
        // https://en.wikipedia.org/wiki/Color_difference#CIE76
        switch (mode)
        {
            case DeltaEMode.CIE76: return Math.Sqrt(Math.Pow(c0.L - c1.L, 2) + Math.Pow(c0.a - c1.a, 2) + Math.Pow(c0.b - c1.b, 2));
            case DeltaEMode.CIE94:
                {
                    double dl = c0.L - c1.L;
                    double ce1 = Math.Sqrt(Math.Pow(c0.a, 2) + Math.Pow(c0.b, 2));
                    double ce2 = Math.Sqrt(Math.Pow(c1.a, 2) + Math.Pow(c1.b, 2));
                    double dc = ce1 - ce2;
                    double dh = Math.Sqrt(Math.Pow(c0.a - c1.a, 2) + Math.Pow(c0.b - c1.b, 2) - Math.Pow(dc, 2));
                    return Math.Sqrt(Math.Pow(dl, 2)
                        + Math.Pow(dc / (1 + 0.045 * ce1), 2)
                        + Math.Pow(dh / (1 + 0.015 * ce2), 2));
                }
            case DeltaEMode.CIEDE2000:
                break;
            case DeltaEMode.CMC_Ic1984:
                break;
        }
        throw new NotImplementedException();
    }
}
public enum DeltaEMode
{
    CIE76,
    CIE94,
    CIEDE2000,
    CMC_Ic1984,
}