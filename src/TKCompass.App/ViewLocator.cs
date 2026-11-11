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
        // 将 ViewModels 命名空间替换为 Views 命名空间，并将 ViewModel 后缀替换为 View
        var name = fullName
            .Replace(".ViewModels.", ".Views.", System.StringComparison.Ordinal)
            .Replace("ViewModel", "View", System.StringComparison.Ordinal);
        var type = System.Type.GetType(name);
        
        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }
    
    public bool Match(object? data)
    {
        return data is ObservableObject;
    }
}
