using StringArtGenerator.App.Resources.Enums;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Windows;

namespace StringArtGenerator.App.Model;

public class Line : LineInstruction
{
    [JsonIgnore] public Nail Nail1 { get; set; }
    [JsonIgnore] public TangentSide Nail1Out { get; set; }
    [JsonIgnore] public Point Nail1RealPos { get; set; }
    [JsonIgnore] public Nail Nail2 { get; set; }
    [JsonIgnore] public TangentSide Nail2In { get; set; }
    [JsonIgnore] public Point Nail2RealPos { get; set; }
    [JsonIgnore] public BresenhamPoint[] Points { get; set; } = Array.Empty<BresenhamPoint>();

    public void Apply(double[,] data, double weight)
    {
        foreach (BresenhamPoint p in Points)
            data[p.Y, p.X] = p.Evaluate(data[p.Y, p.X], weight);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Line l) return false;
        return Nail1 == l.Nail1 && Nail2 == l.Nail2
            || Nail1 == l.Nail2 && Nail2 == l.Nail1;
    }

    public override int GetHashCode()
        => Nail1.GetHashCode() ^ Nail2.GetHashCode();

    public double GetIndicator(MaximizationFunction function, double[,] data, double[,] target, double weight)
        => Points.Sum(p => GetIndicator(function, data[p.Y, p.X], target[p.Y, p.X], p, weight));

    public ThreadInstruction GetInstruction(int idx)
        => new()
        {
            ThreadIndex = idx,
            Nail1Index = Nail1Index,
            Nail2Index = Nail2Index,
            Nail1RealPos = Nail1RealPos,
            Nail2RealPos = Nail2RealPos,
            Nail1Out = Nail1Out,
            Nail2In = Nail2In,
        };

    private static double GetIndicator(MaximizationFunction function, double data, double target, BresenhamPoint p, double weight) => function switch
    {
        MaximizationFunction.DeltaSquare => Math.Pow(data - target, 2) - Math.Pow(p.Evaluate(data, weight) - target, 2),
        MaximizationFunction.Delta => target - data,
        _ => throw new InvalidOperationException(),
    };

    public struct BresenhamPoint
    {
        private readonly float _factor = 1;

        public BresenhamPoint(int x, int y)
        {
            X = (Int16)x;
            Y = (Int16)y;
        }
        public BresenhamPoint(int x, int y, double factor) : this(x, y)
        {
            _factor = (float)factor;
        }

        public Int16 X { get; }
        public Int16 Y { get; }

        public double Evaluate(double value, double weight) => Math.Clamp(value + weight * _factor, 0, 1);
    }
}