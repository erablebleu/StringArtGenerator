using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Adapters;

public class ThreadAdapter : AdapterBase
{
    private Color _color = Colors.Black;
    private double _previewThickness = 0.1;
    private double _calculationThickness = 0.1;
    private BitmapImage? _filteredImage;
    private LuminosityAdjustmentAdapter _luminosityAdjustment = new();
    private ColorAdjustmentAdapter _colorAdjustment = new();
    private int _maxLineCount = 5000;
    private int _stepCount;
    private double _totalLength;
    private double _filterStrength = 1;

    public Color Color { get => _color; set => Set(ref _color, value); }
    public double PreviewThickness { get => _previewThickness; set => Set(ref _previewThickness, value); }
    public double CalculationThickness { get => _calculationThickness; set => Set(ref _calculationThickness, value); }
    public BitmapImage? FilteredImage { get => _filteredImage; set => Set(ref _filteredImage, value); }
    public LuminosityAdjustmentAdapter LuminosityAdjustment { get => _luminosityAdjustment; set => Set(ref _luminosityAdjustment, value); }
    public ColorAdjustmentAdapter ColorAdjustment { get => _colorAdjustment; set => Set(ref _colorAdjustment, value); }
    public int MaxLineCount { get=> _maxLineCount; set => Set(ref _maxLineCount, value); }
    public double FilterStrength { get => _filterStrength; set => Set(ref _filterStrength, value); }
    public int StepCount { get => _stepCount; set => Set(ref _stepCount, value); }
    public double TotalLength { get => _totalLength; set => Set(ref _totalLength, value); }
}