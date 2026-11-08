using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TKCompass.Views;

public partial class CreatorEditWindow : Window
{
    public CreatorEditWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
