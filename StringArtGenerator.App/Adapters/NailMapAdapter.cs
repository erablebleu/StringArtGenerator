using System.Collections.ObjectModel;
using System.Windows;

namespace StringArtGenerator.App.Adapters;

public class NailMapAdapter : AdapterBase
{
    private ObservableCollection<NailAdapter> _nails = new();
    private ObservableCollection<LineAdapter> _lines = new();
    private Point _position = new();
    private double _scale = 1;

    public ObservableCollection<NailAdapter> Nails { get => _nails; set => Set(ref _nails, value); }
    public ObservableCollection<LineAdapter> Lines { get => _lines; set => Set(ref _lines, value); }
    public Point Position { get => _position; set => Set(ref _position, value); }
    public double Scale { get => _scale; set => Set(ref _scale, value); }
}