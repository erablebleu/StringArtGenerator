using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Model;

public class Thread
{
    public Color Color { get; set; } = Colors.Black;
    public int MaxLineCount { get; set; } = 5000;
    public double PreviewThickness { get; set; } = 0.1;
    public double CalculationThickness { get; set; } = 0.1;
    public BitmapImage? FilteredImage { get; set; }
    public LuminosityAdjustment LuminosityAdjustment { get; set; } = new();
    public ColorAdjustment ColorAdjustment { get; set; } = new();
    public double FilterStrength { get; set; } = 1;
    public int StepCount { get; set; }
    public double TotalLength { get; set; }
}