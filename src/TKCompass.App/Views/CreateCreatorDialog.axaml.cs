using Avalonia.Controls;
using Avalonia.Interactivity;
using TKCompass.ViewModels;

namespace TKCompass.Views;

public partial class CreateCreatorDialog : Window
{
    public CreateCreatorDialog()
    {
        InitializeComponent();
    }

    public CreateCreatorDialog(CreateCreatorDialogViewModel viewModel)
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

    private async void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is CreateCreatorDialogViewModel vm)
        {
            var success = await vm.SubmitAsync();
            if (success)
                Close(true);
        }
    }

    public new CreateCreatorDialogViewModel? DataContext
    {
        get => base.DataContext as CreateCreatorDialogViewModel;
        set => base.DataContext = value;
    }
}
