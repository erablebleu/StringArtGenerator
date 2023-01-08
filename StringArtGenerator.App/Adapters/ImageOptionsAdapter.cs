using StringArtGenerator.App.Model;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Adapters;

public abstract class ActivableOptionAdapter : AdapterBase
{
    private bool _isEnabled;
    private BitmapImage? _resultImage;

    public bool IsEnabled { get => _isEnabled; set => Set(ref _isEnabled, value); }
    public BitmapImage? ResultImage { get => _resultImage; set => Set(ref _resultImage, value); }
}
public class SizeOptionAdapter : ActivableOptionAdapter
{
    private double _maxPPMM = 1; // max pixel per milimeter

    public double MaxPPMM { get => _maxPPMM; set => Set(ref _maxPPMM, value); }
}
public class OpacityOptionAdapter : ActivableOptionAdapter
{
    private double _evolutionPow = 1;
    private double _length = 100;

    public double EvolutionPow { get => _evolutionPow; set => Set(ref _evolutionPow, value); }
    public double Length { get => _length; set => Set(ref _length, value); }
}
public class BlurOptionAdapter : ActivableOptionAdapter
{
    private double _evolutionPow = 1;
    private double _length = 100;
    private double _diameter = 20;

    public double EvolutionPow { get => _evolutionPow; set => Set(ref _evolutionPow, value); }
    public double Length { get => _length; set => Set(ref _length, value); }
    public double Diameter { get => _diameter; set => Set(ref _diameter, value); }
}

public class ImageOptionsAdapter : AdapterBase
{
    private BlurOptionAdapter _blurOption = new();
    private OpacityOptionAdapter _opacityOption = new();
    private SizeOptionAdapter _sizeOption = new();

    public BlurOptionAdapter BlurOption { get => _blurOption; set => Set(ref _blurOption, value); }
    public OpacityOptionAdapter OpacityOption { get => _opacityOption; set => Set(ref _opacityOption, value); }
    public SizeOptionAdapter SizeOption { get => _sizeOption; set => Set(ref _sizeOption, value); }
}
