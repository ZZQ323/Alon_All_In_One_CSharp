using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TKCompass.ViewModels;

namespace TKCompass.Views;

public partial class CreatorDetailView : UserControl
{
    public CreatorDetailView()
    {
        InitializeComponent();
    }
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is CreatorDetailViewModel vm)
        {
            vm.OpenShipmentEditRequested -= OnOpenShipmentEditRequested;
            vm.OpenShipmentEditRequested += OnOpenShipmentEditRequested;
            
            vm.OpenVideoEditRequested -= OnOpenVideoEditRequested;
            vm.OpenVideoEditRequested += OnOpenVideoEditRequested;
        }
    }
    
    private async void OnOpenShipmentEditRequested(ShipmentEditViewModel editVm)
    {
        var window = new ShipmentEditWindow
        {
            DataContext = editVm
        };
        
        // 加载数据（编辑模式需要 LoadAsync 填充属性）
        await editVm.LoadAsync();
        
        var parent = VisualRoot as Window;
        if (parent != null)
            await window.ShowDialog(parent);
    }
    
    private async void OnOpenVideoEditRequested(VideoEditViewModel editVm)
    {
        var window = new VideoEditWindow
        {
            DataContext = editVm
        };
        
        var parent = VisualRoot as Window;
        if (parent != null)
            await window.ShowDialog(parent);
    }
}
