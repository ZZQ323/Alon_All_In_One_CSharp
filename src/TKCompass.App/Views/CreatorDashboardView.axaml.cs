using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TKCompass.Views;

public partial class CreatorDashboardView : UserControl
{
    public CreatorDashboardView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
