using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TKCompass.Views;

public partial class CrmView : UserControl
{
    public CrmView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
