namespace StringArtGenerator.App.Adapters;

public class ActivableAdapter : AdapterBase
{
    private bool _isActive;

    public bool IsActive { get => _isActive; set => Set(ref _isActive, value); }
}