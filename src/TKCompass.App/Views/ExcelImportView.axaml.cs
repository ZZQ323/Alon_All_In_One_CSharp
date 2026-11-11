using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using TKCompass.ViewModels;

namespace TKCompass.Views;

public partial class ExcelImportView : UserControl
{
    public ExcelImportView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void OnSelectFileClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ExcelImportViewModel vm)
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择 Excel 文件",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Excel 文件")
                {
                    Patterns = new[] { "*.xlsx", "*.xls" },
                    MimeTypes = new[]
                    {
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "application/vnd.ms-excel"
                    }
                },
                FilePickerFileTypes.All
            }
        });

        if (files.Count > 0)
        {
            // 尝试获取本地路径
            var file = files[0];
            var path = file.TryGetLocalPath() ?? file.Path.LocalPath;
            vm.FilePath = path;
            vm.StatusMessage = $"已选择文件：{System.IO.Path.GetFileName(path)}";
            vm.HasErrors = false;
        }
    }
}
