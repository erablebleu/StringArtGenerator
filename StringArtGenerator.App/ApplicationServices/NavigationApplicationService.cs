using StringArtGenerator.App.ViewModels;
using StringArtGenerator.App.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.App.ApplicationServices;
public class NavigationApplicationService : ApplicationServiceBase
{
    private Dictionary<ViewModelBase, PopupView> _map = new();
    public void ShowPopup(ViewModelBase vm, bool modular = false)
    {
        if (_map.ContainsKey(vm))
        {
            _map[vm].Focus();
        }
        else
        {
            PopupView win = new ()
            {
                DataContext = new PopupViewModel
                {
                    ViewModel = vm
                }
            };
            _map[vm] = win;
            if (modular)
                win.Show();
            else
                win.ShowDialog();
        }
    }
    public void ClosePopup(ViewModelBase vm)
    {
        if (_map.ContainsKey(vm))
            _map[vm].Close();
    }
}
