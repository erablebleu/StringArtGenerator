using StringArtGenerator.App.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StringArtGenerator.App.Views;
/// <summary>
/// Interaction logic for MapperView.xaml
/// </summary>
public partial class MapperView : UserControl
{
    private NailAdapter? _selectedNail;
    public MapperView()
    {
        InitializeComponent();
    }

    private void dgNails_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedNail = dgNails.SelectedItem as NailAdapter;
        ListCollectionView? ls = dgLines.ItemsSource as ListCollectionView;
        ls?.Refresh();
    }

    private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
    {
        if (_selectedNail is null)
            e.Accepted = true;
        else
        {
            LineAdapter? line = e.Item as LineAdapter;

            if (_selectedNail is null)
                e.Accepted = true;
            else
                e.Accepted = line.Nail1 == _selectedNail || line.Nail2 == _selectedNail;
        }
    }
}
