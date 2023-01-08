using StringArtGenerator.App.Adapters.Shape;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StringArtGenerator.App.ViewModels;

public class MapperViewModel : ViewModelBase
{
    private ICommand? _generateMapCommand;
    private ShapeSettingsAdapter? _selectedShapeSettings;

    public ICommand GenerateMapCommand => _generateMapCommand ??= new RelayCommand(OnGenerateMapCommand);
    public ShapeSettingsAdapter? SelectedShapeSettings { get => _selectedShapeSettings; set => Set(ref _selectedShapeSettings, value); }

    public ObservableCollection<ShapeSettingsAdapter> ShapeSettings { get; private set; } = new()
    {
        new PolygonSettingsAdapter(),
        new RectangleSettingsAdapter(),
        new CircleSettingsAdapter(),
    };

    private void OnGenerateMapCommand()
    {
        if (SelectedShapeSettings is null)
            return;

        ProjectApplicationService.CurrentProject.NailMap = SelectedShapeSettings.GetNailMapAdapter(Mapper);
    }
}