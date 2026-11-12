using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using TKCompass.ViewModels;

namespace TKCompass;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null) return null;
        
        var fullName = data.GetType().FullName!;
        var name = fullName
            .Replace(".ViewModels.", ".Views.", System.StringComparison.Ordinal)
            .Replace("ViewModel", "View", System.StringComparison.Ordinal);
        
        // 使用 data 所在程序集解析类型
        var assembly = data.GetType().Assembly;
        var type = assembly.GetType(name);
        
        if (type != null)
        {
            var view = (Control)Activator.CreateInstance(type)!;
            // 显式设置 DataContext，确保 View 能正确绑定到 ViewModel
            view.DataContext = data;
            return view;
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }
    
    public bool Match(object? data)
    {
        return data is ObservableObject;
    }
}
