using AutoMapper;
using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.ApplicationServices;
using System.Windows.Input;

namespace StringArtGenerator.App.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private ICommand _cancelCommand;
    private ICommand _saveCommand;
    private SettingsAdapter _settings;

    public SettingsViewModel(IMapper mapper, SettingsApplicationService settingsApplicationService)
    {
        _settings = mapper.Map<SettingsAdapter>(settingsApplicationService.Settings);
    }

    public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(OnCancelCommand);
    public ICommand SaveCommand => _saveCommand ??= new RelayCommand(OnSaveCommand);
    public SettingsAdapter Settings { get => _settings; private set => Set(ref _settings, value); }

    private void OnCancelCommand()
    {
        NavigationApplicationService.ClosePopup(this);
    }
    private void OnSaveCommand()
    {
        SettingsApplicationService.Save(Settings);
        OnCancelCommand();
    }
}