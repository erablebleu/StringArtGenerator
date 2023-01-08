namespace StringArtGenerator.App.Adapters;

public class ColorAdjustmentAdapter : ActivableAdapter
{
    private double[,] _colorMatrix = new double[3, 3]
    {
        {0.30d, 0.30d, 0.30d },
        {0.59d, 0.59d, 0.59d },
        {0.11d, 0.11d, 0.11d },
    };
    
    public double [,] ColorMatrix { get => _colorMatrix; set => Set(ref _colorMatrix, value); }
    public double ColorValue00 { get => _colorMatrix[0, 0]; set => Set(ref _colorMatrix[0, 0], value); }
    public double ColorValue01 { get => _colorMatrix[0, 1]; set => Set(ref _colorMatrix[0, 1], value); }
    public double ColorValue02 { get => _colorMatrix[0, 2]; set => Set(ref _colorMatrix[0, 2], value); }
    public double ColorValue10 { get => _colorMatrix[1, 0]; set => Set(ref _colorMatrix[1, 0], value); }
    public double ColorValue11 { get => _colorMatrix[1, 1]; set => Set(ref _colorMatrix[1, 1], value); }
    public double ColorValue12 { get => _colorMatrix[1, 2]; set => Set(ref _colorMatrix[1, 2], value); }
    public double ColorValue20 { get => _colorMatrix[2, 0]; set => Set(ref _colorMatrix[2, 0], value); }
    public double ColorValue21 { get => _colorMatrix[2, 1]; set => Set(ref _colorMatrix[2, 1], value); }
    public double ColorValue22 { get => _colorMatrix[2, 2]; set => Set(ref _colorMatrix[2, 2], value); }
}