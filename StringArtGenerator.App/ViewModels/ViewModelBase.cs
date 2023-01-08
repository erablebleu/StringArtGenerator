using StringArtGenerator.App.ApplicationServices;
using StringArtGenerator.Common;

namespace StringArtGenerator.App.ViewModels;

public abstract class ViewModelBase : ApplicationServiceBase
{
    [Injectable] public ProjectApplicationService ProjectApplicationService { get; set; }
    [Injectable] public SettingsApplicationService SettingsApplicationService { get; set; }
    [Injectable] public NavigationApplicationService NavigationApplicationService { get; set; }
    [Injectable] public CalculatorApplicationService CalculatorApplicationService { get; set; }
    [Injectable] public ILoader Loader { get; set; }

    public virtual void Load()
    {
    }
}