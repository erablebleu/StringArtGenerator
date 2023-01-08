using StringArtGenerator.App.Resources.Enums;

namespace StringArtGenerator.App.Model;

public class CalculationOptions
{
    public bool AntiReverse { get; set; }
    public int AntiReverseQueueSize { get; set; } = 20;
    public bool Continuity { get; set; } = true;
    public MaximizationFunction MaximizationFunction { get; set; } = MaximizationFunction.Delta;
    public bool UseBresenham { get; set; }
    public bool FinalRevers { get; set; }
    public bool UseLabColorDistance { get; set; }
}