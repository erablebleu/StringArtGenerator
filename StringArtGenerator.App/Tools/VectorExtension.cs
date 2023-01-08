using System;
using System.Windows;

namespace StringArtGenerator.App.Tools;

public static class VectorExtension
{
    public static Vector Rotate(this Vector v, double a)
    {
        double c = Math.Cos(a),
               s = Math.Sin(a);
        return new Vector(c * v.X - s * v.Y, s * v.X + c * v.Y);
    }
}