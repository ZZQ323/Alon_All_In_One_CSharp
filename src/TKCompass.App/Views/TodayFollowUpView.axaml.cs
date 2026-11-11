using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TKCompass.Views;

public partial class TodayFollowUpView : UserControl
{
    public TodayFollowUpView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
