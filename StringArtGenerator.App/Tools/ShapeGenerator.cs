using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StringArtGenerator.App.Tools;
public static class ShapeGenerator
{
    public static IEnumerable<Point> GetCircle(int nailCount, double diameter)
    {
        double radius = diameter / 2;
        for (int i = 0; i < nailCount; i++)
        {
            double teta = 2 * Math.PI * i / nailCount;
            yield return new Point(radius * (1 + Math.Sin(teta))/2, radius * (1- Math.Cos(teta))/2);
        }
        yield break;
    }

    public static IEnumerable<Point> GetPolygon(int edgeCount, int edgeNailCount, bool excludeVertex, double diameter = 100, bool topEdge = true)
    {
        List<Point> result = new ();
        double radius = diameter / 2;

        for (int i = 0; i < edgeCount; i++)
        {
            double t1 = 2 * Math.PI * i / edgeCount + (topEdge ? 0 : (Math.PI / edgeCount));
            double t2 = t1 + 2 * Math.PI / edgeCount;
            Point p1 = new(radius * (1 - Math.Sin(t1)), radius * (1 - Math.Cos(t1)));
            Point p2 = new(radius * (1 - Math.Sin(t2)), radius * (1 - Math.Cos(t2)));
            result.AddRange(GetLine(p1, p2, edgeNailCount, excludeVertex));
        }

        double minX = result.Min(p => p.X);
        double minY = result.Min(p => p.Y);

        return result.Select(p => new Point(p.X - minX, p.Y - minY));
    }

    public static IEnumerable<Point> GetRectangle(int xNailCount, int yNailCount, double width, double height, bool excludeVertex)
    {
        List<Point> result = new();

        result.AddRange(GetLine(new Point(0, 0), new Point(width, 0), xNailCount, excludeVertex));
        result.AddRange(GetLine(new Point(width, 0), new Point(width, height), yNailCount, excludeVertex));
        result.AddRange(GetLine(new Point(width, height), new Point(0, height), xNailCount, excludeVertex));
        result.AddRange(GetLine(new Point(0, height), new Point(0, 0), yNailCount, excludeVertex));

        return result;
    }

    public static IEnumerable<Point> GetLine(Point p1, Point p2, int cnt, bool excludeVertex)
    {
        Vector v = (p2 - p1) / cnt;
        if (excludeVertex)
            p1 += v / 2;
        for (int i = 0; i < cnt; i++)
        {

            yield return new Point(p1.X, p1.Y);
            p1 += v;
        }
        yield break;
    }
}
