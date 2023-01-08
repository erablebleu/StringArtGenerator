namespace StringArtGenerator.App.Adapters;

public class StepperAdapter : AdapterBase
{
    private int _step = 1;
    public int Step { get => _step; set => Set(ref _step, value); }
}