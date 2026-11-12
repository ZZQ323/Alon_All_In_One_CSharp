using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using TKCompass.Services;
using TKCompass.ViewModels;

namespace TKCompass.Views;

public partial class ConfigView : UserControl
{
    public ConfigView()
    {
        InitializeComponent();

        // 注册 DataGrid 双击事件
        this.AddHandler(InputElement.DoubleTappedEvent, OnDataGridDoubleTapped, RoutingStrategies.Tunnel);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// 获取当前顶层窗口，用于作为对话框的 owner
    /// </summary>
    private Window? GetParentWindow()
    {
        // 先尝试从可视化树向上查找
        var visual = this.VisualRoot as Window;
        if (visual != null) return visual;

        // 回退到应用程序当前活动窗口
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                   ?? desktop.MainWindow;
        }
        return null;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is ConfigViewModel vm)
        {
            vm.ShowEditDialogAsync = ShowEditDialogAsync;
        }
    }

    private async Task<bool> ShowEditDialogAsync(ConfigEditContext context)
    {
        var dialogVm = new ConfigEditDialogViewModel
        {
            Name = context.Label,
            Code = context.Code,
            Description = context.Description,
            SortOrder = context.SortOrder,
            IsEditMode = context.IsEdit,
            ShowCode = context.ShowCodeField,
            ShowSortOrder = context.ShowSortOrderField,
            NameWatermark = context.ShowCodeField ? "名称" : "名称",
            CodeWatermark = "代码",
            DescriptionWatermark = context.ShowCodeField ? "描述" : "描述"
        };

        var dialog = new ConfigEditDialog(dialogVm);
        dialog.Title = context.Title;

        // 设置 Watermark 绑定在 Avalonia 中需要通过代码设置
        // 对于 CreatorType，不需要 Description 字段，但对话框已有，所以显示即可
        var result = await dialog.ShowDialog<bool>(GetParentWindow()!);

        if (result)
        {
            // 将对话框中的值写回 Context
            context.Label = dialogVm.Name;
            context.Code = dialogVm.Code;
            context.Description = dialogVm.Description;
            context.SortOrder = dialogVm.SortOrder;
        }

        return result;
    }

    private void OnDataGridDoubleTapped(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ConfigViewModel vm) return;

        // 检查是否点击在 DataGridRow 上
        var source = e.Source as Control;
        while (source != null && source is not DataGridRow)
        {
            source = source.Parent as Control;
        }

        if (source is DataGridRow)
        {
            // 根据当前选中的 Tab 执行对应的编辑命令
            var dataGrid = FindParentDataGrid(source);
            if (dataGrid == null) return;

            var dataGridName = dataGrid.Name;
            if (dataGridName == "CreatorTypeDataGrid" && vm.EditCreatorTypeCommand.CanExecute(null))
            {
                vm.EditCreatorTypeCommand.Execute(null);
            }
            else if (dataGridName == "ProductCodeDataGrid" && vm.EditProductCodeCommand.CanExecute(null))
            {
                vm.EditProductCodeCommand.Execute(null);
            }
            else if (dataGridName == "QualityLevelDataGrid" && vm.EditQualityLevelCommand.CanExecute(null))
            {
                vm.EditQualityLevelCommand.Execute(null);
            }
            else if (dataGridName == "ContactChannelDataGrid" && vm.EditContactChannelCommand.CanExecute(null))
            {
                vm.EditContactChannelCommand.Execute(null);
            }
        }
    }

    private DataGrid? FindParentDataGrid(Control? control)
    {
        while (control != null)
        {
            if (control is DataGrid dg) return dg;
            control = control.Parent as Control;
        }
        return null;
    }
}
