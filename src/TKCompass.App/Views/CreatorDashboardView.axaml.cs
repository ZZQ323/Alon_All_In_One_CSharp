using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using TKCompass.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace TKCompass.Views;

public partial class CreatorDashboardView : UserControl
{
    public CreatorDashboardView()
    {
        InitializeComponent();
    }

    private async void CreateCreatorButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CreatorDashboardViewModel vm) return;
        var window = this.VisualRoot as Window;
        if (window == null) return;

        // 从 DI 容器获取服务
        var creatorService = App.Services?.GetService<ICreatorService>();
        var messenger = App.Services?.GetService<IMessenger>();
        if (creatorService == null || messenger == null) return;

        var dialogVm = new CreateCreatorDialogViewModel(creatorService, messenger);
        var dialog = new CreateCreatorDialog(dialogVm);
        var result = await dialog.ShowDialog<bool>(window);
        if (result)
            await vm.RefreshCommand.ExecuteAsync(null);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Selection handling moved to AllCreatorsView
    }

    private void DeliveryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is CreatorDashboardViewModel vm && sender is DataGrid dataGrid)
        {
            vm.SelectedDeliveryCreators.Clear();
            foreach (var item in dataGrid.SelectedItems)
            {
                if (item is Creator creator)
                {
                    vm.SelectedDeliveryCreators.Add(creator);
                }
            }
        }
    }

    private void DataGrid_DoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is CreatorDashboardViewModel vm && sender is DataGrid dataGrid)
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
