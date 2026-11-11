using Avalonia.Controls;
using Avalonia.Controls.Templates;
using TKCompass.ViewModels;

namespace TKCompass;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null) return null;
        
        var name = data.GetType().FullName!
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
        return data is ViewModelBase;
    }
}
