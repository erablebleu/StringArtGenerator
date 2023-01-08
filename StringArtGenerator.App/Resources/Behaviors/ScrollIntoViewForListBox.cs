using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace StringArtGenerator.App.Resources.Behaviors;

public class ScrollIntoViewForListBox : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
    }

    private void AssociatedObject_SelectionChanged(object sender,
                                               SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox) return;

        if (listBox.SelectedItem == null)
            return;

        listBox.Dispatcher.BeginInvoke(() =>
            {
                listBox.UpdateLayout();
                if (listBox.SelectedItem != null)
                    listBox.ScrollIntoView(listBox.SelectedItem);
            });
    }
}