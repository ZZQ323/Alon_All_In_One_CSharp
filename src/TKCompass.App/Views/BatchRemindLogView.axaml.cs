using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TKCompass.Views;

public partial class BatchRemindLogView : UserControl
{
    public BatchRemindLogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
