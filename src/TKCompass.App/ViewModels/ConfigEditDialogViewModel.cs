using CommunityToolkit.Mvvm.ComponentModel;

namespace TKCompass.ViewModels;

/// <summary>
/// 配置编辑弹窗 ViewModel
/// </summary>
public partial class ConfigEditDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private int _sortOrder = 0;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _showCode;

    [ObservableProperty]
    private bool _showSortOrder;

    [ObservableProperty]
    private string _codeWatermark = "代码";

    [ObservableProperty]
    private string _nameWatermark = "名称";

    [ObservableProperty]
    private string _descriptionWatermark = "描述";

    /// <summary>
    /// 配置类型：CreatorType, ProductCode, QualityLevel, ContactChannel
    /// </summary>
    public string ConfigType { get; set; } = string.Empty;

    /// <summary>
    /// 编辑模式下的原始代码（用于 Dictionary 类型）
    /// </summary>
    public string? OriginalCode { get; set; }

    /// <summary>
    /// 编辑模式下的原始类型（用于 Dictionary 类型）
    /// </summary>
    public string? OriginalType { get; set; }

    /// <summary>
    /// 编辑模式下的原始 ID（用于 CreatorType, ProductCode）
    /// </summary>
    public int? OriginalId { get; set; }
}
