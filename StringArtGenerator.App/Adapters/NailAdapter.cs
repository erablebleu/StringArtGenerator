using System.Windows;

namespace StringArtGenerator.App.Adapters;

public class NailAdapter : AdapterBase
{
    private Point _position = new();
    private double _diameter;

    public Point Position { get => _position; set => Set(ref _position, value); }
    public double Diameter { get => _diameter; set => Set(ref _diameter, value); }
}