namespace StringArtGenerator.App.Adapters;

public class LineAdapter : AdapterBase
{
    private NailAdapter _nail1;
    private NailAdapter _nail2;

    public NailAdapter Nail1 { get => _nail1; set => Set(ref _nail1, value); }
    public NailAdapter Nail2 { get => _nail2; set => Set(ref _nail2, value); }
}