using StringArtGenerator.App.Resources.Enums;
using StringArtGenerator.App.Resources.Extensions;
using StringArtGenerator.App.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StringArtGenerator.App.Model;

public class LineFactory
{
    private readonly double _mapScale;
    private readonly bool _useBresenham;
    private readonly int _bitmapWidth;
    private readonly int _bitmapHeight;
    private readonly List<Nail> _nails;

    public LineFactory(bool useBresenham, double mapScale, List<Nail> nails, int bitmapWidth, int bitmapHeight)
    {
        _useBresenham = useBresenham;
        _mapScale = mapScale;
        _bitmapWidth = bitmapWidth;
        _bitmapHeight = bitmapHeight;
        _nails = nails;
    }

    public Line GetLine(int nail1Idx, int nail2Idx, Nail nail1, Nail nail2, TangentSide nail1Out, TangentSide nail2In)
    {
        Line result = new()
        {
            Nail1Index = nail1Idx,
            Nail2Index = nail2Idx,
            Nail1 = nail1,
            Nail2 = nail2,
            Nail1Out = nail1Out,
            Nail2In = nail2In,
        };

        (result.Nail1RealPos, result.Nail2RealPos) = GetTangentPoint(nail1.Position, nail2.Position, nail1.Diameter / 2, nail2.Diameter / 2, nail1Out, nail2In);

        result.Points = _useBresenham ? GetBresenhamLine(result.Nail1RealPos.Scale(_mapScale), 
                                                         result.Nail2RealPos.Scale(_mapScale))
                                      : GetSimpleLine(result.Nail1RealPos.Scale(_mapScale), 
                                                      result.Nail2RealPos.Scale(_mapScale));

        // Exclude points outside of bitmap
        result.Points = result.Points.Where(p => p.X > 0 && p.Y > 0 && p.X < _bitmapWidth && p.Y < _bitmapHeight).ToArray();

        return result;
    }

    public Line[] GetLines(IEnumerable<int> targets, int nail1Idx, TangentSide nail1Out)
        => targets.Select(j => GetLine(nail1Idx, j, _nails[nail1Idx], _nails[j], nail1Out, TangentSide.Left))
   .Concat(targets.Select(j => GetLine(nail1Idx, j, _nails[nail1Idx], _nails[j], nail1Out, TangentSide.Right))).ToArray();

    private static Line.BresenhamPoint[] GetBresenhamLine(Point p0, Point p1) => GetBresenhamLine((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y);

    private static Line.BresenhamPoint[] GetBresenhamLine(int x0, int y0, int x1, int y1)
    {
        static double fPartOfNumber(double x) => (x > 0) ? x - (int)x : x - (int)x - 1;

        List<Line.BresenhamPoint> points = new();

        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        // swap the co-ordinates if slope > 1 or we
        // draw backwards
        if (steep)
            (x0, y0, x1, y1) = (y0, x0, y1, x1);
        if (x0 > x1)
            (x0, y0, x1, y1) = (x1, y1, x0, y0);

        //compute the slope
        double dx = x1 - x0;
        double dy = y1 - y0;

        // main loop
        for (int x = x0; x <= x1; x++)
        {
            // pixel coverage is determined by fractional
            // part of y co-ordinate
            double y = y0 + (x - x0) * dy / dx;

            if (1 - fPartOfNumber(y) > 0)
            {
                points.Add(steep ? new Line.BresenhamPoint((int)y, x, 1 - fPartOfNumber(y)) 
                                 : new Line.BresenhamPoint(x, (int)y, 1 - fPartOfNumber(y)));
            }
            if (y - 1 < 0) continue;
            if (fPartOfNumber(y) > 0)
            {
                points.Add(steep ? new Line.BresenhamPoint((int)y - 1, x, fPartOfNumber(y)) 
                                 : new Line.BresenhamPoint(x, (int)y - 1, fPartOfNumber(y)));
            }
        }

        return points.ToArray();
    }

    private static Line.BresenhamPoint[] GetSimpleLine(Point p0, Point p1) => GetSimpleLine(p0.X, p0.Y, p1.X, p1.Y);

    private static Line.BresenhamPoint[] GetSimpleLine(double x0, double y0, double x1, double y1)
    {
        double dx = x1 - x0;
        double dy = y1 - y0;
        double d = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        int count = (int)Math.Floor(d);
        return Enumerable.Range(0, count)
                         .Select(i => new Line.BresenhamPoint((int)(x0 + dx * i / (count - 1)),
                                                              (int)(y0 + dy * i / (count - 1)))).ToArray();
    }

    private static (Point, Point) GetTangentPoint(Point c1, Point c2, double r, TangentSide t1, TangentSide t2)
    {
        if (r == 0)
            return (c1, c2);

        Vector x = c2 - c1;
        double d = x.Length;
        x.Normalize();
        Vector y = x.Rotate(-Math.PI / 2);

        if (t1 == t2)
        {
            if (t1 == TangentSide.Right)
                r = -r;

            return (c1 + r * y, c2 + r * y);
        }
        else
        {
            double a = Math.Acos(2 * r / d) * (t1 == TangentSide.Right ? 1 : -1);

            return (c1 + r * x.Rotate(a), c2 + r * (-x).Rotate(a));
        }
    }

    private static (Point, Point) GetTangentPoint(Point c1, Point c2, double r1, double r2, TangentSide t1, TangentSide t2)
    {
        Vector x = c2 - c1;
        double d = x.Length;
        x.Normalize();

        if (t1 == t2)
        {
            double a = Math.Acos((r1 - r2) / d) * (t1 == TangentSide.Right ? 1 : -1);

            return (c1 + r1 * x.Rotate(a), c2 + r2 * x.Rotate(a));
        }
        else
        {
            double a = Math.Acos((r1 + r2) / d) * (t1 == TangentSide.Right ? 1 : -1);

            return (c1 + r1 * x.Rotate(a), c2 + r2 * (-x).Rotate(a));
        }
    }
}