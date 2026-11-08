using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TKCompass.Views;

public partial class ShipmentEditWindow : Window
{
    public ShipmentEditWindow()
    {
        InitializeComponent();
    }
    
    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
