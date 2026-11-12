using Avalonia;
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

public partial class CreatorDashboardView : UserControl, IRecipient<OpenCreateCreatorDialogMessage>
{
    public CreatorDashboardView()
    {
        InitializeComponent();
        
        // 订阅打开新建达人弹窗消息
        WeakReferenceMessenger.Default.Register<OpenCreateCreatorDialogMessage>(this);
    }

    public async void Receive(OpenCreateCreatorDialogMessage message)
    {
        if (DataContext is not CreatorDashboardViewModel vm) 
        {
            await ShowErrorDialog("ViewModel 未初始化");
            return;
        }
        
        var window = this.VisualRoot as Window;
        if (window == null) 
        {
            await ShowErrorDialog("无法获取父窗口");
            return;
        }

        // 从 DI 容器获取服务
        var creatorService = App.Services?.GetService<ICreatorService>();
        var messenger = App.Services?.GetService<IMessenger>();
        if (creatorService == null || messenger == null) 
        {
            await ShowErrorDialog("服务未注册，请检查依赖注入配置");
            return;
        }

        var dialogVm = new CreateCreatorDialogViewModel(creatorService, messenger);
        var dialog = new CreateCreatorDialog(dialogVm);
        var result = await dialog.ShowDialog<bool>(window);
        if (result)
            await vm.RefreshCommand.ExecuteAsync(null);
    }

    private async Task ShowErrorDialog(string message)
    {
        var errorWindow = new Window
        {
            Title = "错误",
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            CanResize = false
        };
        
        var stackPanel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 15
        };
        
        stackPanel.Children.Add(new TextBlock 
        { 
            Text = message, 
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        });
        
        var okButton = new Button 
        {
            Content = "确定",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Width = 100
        };
        okButton.Click += (s, e) => errorWindow.Close();
        stackPanel.Children.Add(okButton);
        
        errorWindow.Content = stackPanel;
        
        var parentWindow = this.VisualRoot as Window;
        if (parentWindow != null)
            await errorWindow.ShowDialog(parentWindow);
        else
            errorWindow.Show();
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
