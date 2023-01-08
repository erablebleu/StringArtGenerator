using StringArtGenerator.App.Resources.Enums;
using System;
using System.Text.Json.Serialization;
using System.Windows;

namespace StringArtGenerator.App.Model;

public class Nail
{
    public double Diameter { get; set; } // in mm
    public Point Position { get; set; }
    [JsonIgnore] public Line[] LinesL { get; set; } = Array.Empty<Line>();
    [JsonIgnore] public Line[] LinesR { get; set; } = Array.Empty<Line>();
    [JsonIgnore] public Line[] Lines { get; set; } = Array.Empty<Line>();

    public Line[] GetLines(TangentSide inSide) => inSide switch
    {
        TangentSide.Left => LinesL,
        TangentSide.Right => LinesR,
        _ => Lines,
    };

    public override bool Equals(object? obj)
    {
        return obj is Nail n && n.Position.Equals(Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}