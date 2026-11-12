using Avalonia.Controls;
using TKCompass.ViewModels;

namespace TKCompass.Views;

public partial class InDeliveryView : UserControl
{
    public InDeliveryView()
    {
        InitializeComponent();
    }

    private void DataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is InDeliveryViewModel vm && sender is DataGrid dg)
        {
            vm.SelectedCreators = new System.Collections.ObjectModel.ObservableCollection<TKCompass.Domain.Models.Creator>(
                dg.SelectedItems?.Cast<TKCompass.Domain.Models.Creator>() ?? Array.Empty<TKCompass.Domain.Models.Creator>());
        }
    }
}
