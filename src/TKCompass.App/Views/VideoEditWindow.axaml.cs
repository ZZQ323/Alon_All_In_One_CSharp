using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TKCompass.Views;

public partial class VideoEditWindow : Window
{
    public VideoEditWindow()
    {
        InitializeComponent();
    }
    
    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
