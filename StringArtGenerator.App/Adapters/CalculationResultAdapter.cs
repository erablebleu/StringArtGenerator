using StringArtGenerator.App.Model;
using System.Collections.ObjectModel;

namespace StringArtGenerator.App.Adapters;

public class CalculationResultAdapter : AdapterBase
{
    private ObservableCollection<ThreadInstruction> _instructions = new();
    private double _progress;
    private string _step;

    public ObservableCollection<ThreadInstruction> Instructions { get => _instructions; set => Set(ref _instructions, value); }
    public double Progress { get => _progress; set => Set(ref _progress, value); }
    public string  Step { get => _step; set => Set(ref _step, value); }

    public void StartLocalProgress(double length)
    {
        _localProgressStart = Progress;
        _localProgressLength = length;
    }

    public void SetLocalProgress(double progress, string? step = null)
        => SetProgress(_localProgressStart + progress * _localProgressLength, step);
    public void SetProgress(double progress, string? step = null)
    {
        Progress = progress;
        if (step is not null)
            Step = step;
    }
    public void SetStep(string step)
    {
        Step = step;
    }

    private double _localProgressStart;
    private double _localProgressLength;
}