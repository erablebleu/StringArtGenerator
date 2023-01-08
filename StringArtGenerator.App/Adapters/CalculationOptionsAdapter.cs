using StringArtGenerator.App.Model;
using StringArtGenerator.App.Resources.Enums;

namespace StringArtGenerator.App.Adapters;

public class CalculationOptionsAdapter : AdapterBase
{
    private bool _antiReverse = true;
    private int _antiReverseQueueSize = 20;
    private bool _continuity = true;
    private bool _useBresenham = true;
    private MaximizationFunction _maximizationFunction;
    private bool _finalReverse;
    private bool _useLabColorDistance;

    public bool AntiReverse { get => _antiReverse; set => Set(ref _antiReverse, value); }
    public int AntiReverseQueueSize { get => _antiReverseQueueSize; set => Set(ref _antiReverseQueueSize, value); }
    public bool Continuity { get => _continuity; set => Set(ref _continuity, value); }
    public bool UseBresenham { get => _useBresenham; set => Set(ref _useBresenham, value); }
    public MaximizationFunction MaximizationFunction { get => _maximizationFunction; set => Set(ref _maximizationFunction, value); }
    public bool FinalRevers { get => _finalReverse; set => Set(ref _finalReverse, value); }
    public bool UseLabColorDistance { get => _useLabColorDistance; set => Set(ref _useLabColorDistance, value); }
}