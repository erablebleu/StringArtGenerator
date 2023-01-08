using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.ApplicationServices;
using StringArtGenerator.App.Tools;
using StringArtGenerator.Common;
using System.Windows.Input;

namespace StringArtGenerator.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ICommand? _changeSourceImageCommand;
    private ICommand? _exportGifCommand;
    private ICommand? _newCommand;
    private ICommand? _openCommand;
    private ICommand? _openSettingsCommand;
    private ICommand? _saveAsCommand;
    private ICommand? _saveCommand;
    public ICommand ChangeSourceImageCommand => _changeSourceImageCommand ??= new RelayCommand(OnChangeSourceImageCommand);
    public ICommand ExportGifCommand => _exportGifCommand ??= new RelayCommand(OnExportGifCommand);
    [Injectable] public MapperViewModel MapperViewModel { get; set; }
    public ICommand NewCommand => _newCommand ??= new RelayCommand(OnNewCommand);
    public ICommand OpenCommand => _openCommand ??= new RelayCommand(OnOpenCommand);
    public ICommand OpenSettingsCommand => _openSettingsCommand ??= new RelayCommand(OnOpenSettingsCommand);
    public ICommand SaveAsCommand => _saveAsCommand ??= new RelayCommand(OnSaveAsCommand/*, () => ProjectApplicationService.CurrentProject.FilePath is not null*/);
    public ICommand SaveCommand => _saveCommand ??= new RelayCommand(OnSaveCommand/*, () => ProjectApplicationService.CurrentProject.FilePath is null || ProjectApplicationService.CurrentProject.IsDirty*/);
    [Injectable] public StepperViewModel StepperViewModel { get; set; }
    [Injectable] public TimeLineViewModel TimeLineViewModel { get; set; }

    public override void Load()
    {
    }

    private void OnChangeSourceImageCommand()
    {
        Microsoft.Win32.OpenFileDialog ofd = new()
        {
            Filter = "Image file|*.bmp;*.jpg;*jpeg",
        };
        if (!ofd.ShowDialog() == true) return;

        ProjectApplicationService.CurrentProject.SourceImage = new System.Drawing.Bitmap(ofd.FileName).ToBitmapImage();
    }

    private void OnExportGifCommand()
    {
        GifExtension.ExportGif($"{ProjectApplicationService.CurrentProject.FilePath}.gif",
                               ProjectApplicationService.CurrentProject.CalculationResult.Instructions);
    }

    private void OnNewCommand()
    {
        ProjectApplicationService.CurrentProject = new ProjectAdapter();
    }

    private void OnOpenCommand()
    {
        Microsoft.Win32.OpenFileDialog ofd = new()
        {
            Filter = "Fichiers Json (.json)|*.json",
        };
        if (!ofd.ShowDialog() == true) return;
        ProjectApplicationService.Open(ofd.FileName);
    }

    private void OnOpenSettingsCommand()
    {
        NavigationApplicationService.ShowPopup(Loader.Resolve<SettingsViewModel>());
    }

    private void OnSaveAsCommand()
    {
        Microsoft.Win32.SaveFileDialog sfd = new()
        {
            Filter = "Fichiers Json (.json)|*.json",
        };
        if (!sfd.ShowDialog() == true) return;
        ProjectApplicationService.Save(sfd.FileName);
    }

    private void OnSaveCommand()
    {
        if (!string.IsNullOrEmpty(ProjectApplicationService.CurrentProject.FilePath))
            ProjectApplicationService.Save(ProjectApplicationService.CurrentProject.FilePath);
        else
        {
            Microsoft.Win32.SaveFileDialog sfd = new()
            {
                Filter = "Fichiers Json (.json)|*.json",
            };
            if (!sfd.ShowDialog() == true) return;
            ProjectApplicationService.Save(sfd.FileName);
        }
    }
}