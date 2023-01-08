using System.Windows;

namespace StringArtGenerator.App.Resources.Extensions;

public static class PointExtension
{
    public static Point Scale(this Point a, double scale, Point offset) => new(offset.X + scale * a.X, offset.Y + scale * a.Y);
    public static Point Scale(this Point a, double scale) => Scale(a, scale, new Point(0, 0));
}