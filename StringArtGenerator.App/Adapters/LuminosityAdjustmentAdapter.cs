namespace StringArtGenerator.App.Adapters;

public class LuminosityAdjustmentAdapter : ActivableAdapter
{
    private double _brightness = 0;
    private double _constrast = 1;

    public double Brightness { get => _brightness; set => Set(ref _brightness, value); }
    public double Contrast { get => _constrast; set => Set(ref _constrast, value); }
}