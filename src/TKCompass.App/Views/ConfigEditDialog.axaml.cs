using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using TKCompass.ViewModels;

namespace TKCompass.Views;

public partial class ConfigEditDialog : Window
{
    public ConfigEditDialog()
    {
        InitializeComponent();
    }

    public ConfigEditDialog(ConfigEditDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        var cancelButton = this.FindControl<Button>("CancelButton");
        var okButton = this.FindControl<Button>("OkButton");

        if (cancelButton != null)
            cancelButton.Click += CancelButton_Click;
        if (okButton != null)
            okButton.Click += OkButton_Click;
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    public new ConfigEditDialogViewModel? DataContext
    {
        get => base.DataContext as ConfigEditDialogViewModel;
        set => base.DataContext = value;
    }
}
