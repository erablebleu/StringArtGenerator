using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.App.ViewModels;
public class PopupViewModel : ViewModelBase
{
    private ViewModelBase _viewModel;

    public ViewModelBase ViewModel { get => _viewModel; set => Set(ref _viewModel, value); }
}
