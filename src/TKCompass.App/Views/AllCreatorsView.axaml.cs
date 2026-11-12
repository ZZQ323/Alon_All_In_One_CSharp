using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TKCompass.Domain.Models;
using TKCompass.ViewModels;

namespace TKCompass.Views;

public partial class AllCreatorsView : UserControl
{
    public AllCreatorsView()
    {
        InitializeComponent();
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is AllCreatorsViewModel vm && sender is DataGrid dataGrid)
        {
            vm.SelectedCreators.Clear();
            foreach (var item in dataGrid.SelectedItems)
            {
                if (item is Creator creator)
                {
                    vm.SelectedCreators.Add(creator);
                }
            }
        }
    }

    private void DataGrid_DoubleTapped(object sender, RoutedEventArgs e)
    {
        if (DataContext is AllCreatorsViewModel vm && sender is DataGrid dataGrid)
        {
            var clickedRow = e.Source as Control;
            while (clickedRow != null && clickedRow is not DataGridRow)
            {
                clickedRow = clickedRow.Parent as Control;
            }

            if (clickedRow is DataGridRow row && row.DataContext is Creator creator)
            {
                vm.OpenCreatorDetailCommand.Execute(creator);
            }
        }
    }
}
