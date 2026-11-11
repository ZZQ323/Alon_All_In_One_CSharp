using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace TKCompass.Services;

/// <summary>
/// 统一通知服务接口
/// </summary>
public interface INotificationService
{
    Task ShowInfoAsync(string title, string message);
    Task ShowWarningAsync(string title, string message);
    Task ShowErrorAsync(string title, string message);
    Task<bool> ConfirmAsync(string title, string message);
}

/// <summary>
/// 基于 Avalonia Window 的通知服务实现
/// </summary>
public class NotificationService : INotificationService
{
    private Func<Window>? _windowResolver;

    /// <summary>
    /// 设置窗口解析器，用于获取当前活动窗口以作为对话框的宿主
    /// </summary>
    public void SetWindowResolver(Func<Window> resolver)
    {
        _windowResolver = resolver;
    }

    private Window? GetParentWindow()
    {
        try
        {
            return _windowResolver?.Invoke();
        }
        catch
        {
            return null;
        }
    }

    public async Task ShowInfoAsync(string title, string message)
    {
        await ShowDialogAsync(title, message, "信息", Colors.DodgerBlue);
    }

    public async Task ShowWarningAsync(string title, string message)
    {
        await ShowDialogAsync(title, message, "警告", Colors.Orange);
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        await ShowDialogAsync(title, message, "错误", Colors.OrangeRed);
    }

    public async Task<bool> ConfirmAsync(string title, string message)
    {
        var window = GetParentWindow();
        if (window == null)
            return false;

        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 15
        };

        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 380
        };
        panel.Children.Add(textBlock);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 10
        };

        var okButton = new Button
        {
            Content = "确定",
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        okButton.Click += (_, _) =>
        {
            dialog.Close(true);
        };

        var cancelButton = new Button
        {
            Content = "取消",
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cancelButton.Click += (_, _) =>
        {
            dialog.Close(false);
        };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);
        panel.Children.Add(buttonPanel);

        dialog.Content = panel;

        var dialogResult = await dialog.ShowDialog<bool>(window);
        return dialogResult;
    }

    private async Task ShowDialogAsync(string title, string message, string severityLabel, Color accentColor)
    {
        var window = GetParentWindow();
        if (window == null)
        {
            System.Diagnostics.Debug.WriteLine($"[{severityLabel}] {title}: {message}");
            return;
        }

        var dialog = new Window
        {
            Title = $"{title} - {severityLabel}",
            Width = 420,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 15
        };

        var textBlock = new TextBlock
        {
            Text = $"[{severityLabel}] {message}",
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 380,
            Foreground = new SolidColorBrush(accentColor)
        };
        panel.Children.Add(textBlock);

        var closeButton = new Button
        {
            Content = "关闭",
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        closeButton.Click += (_, _) =>
        {
            dialog.Close();
        };
        panel.Children.Add(closeButton);

        dialog.Content = panel;

        await dialog.ShowDialog<object>(window);
    }
}
