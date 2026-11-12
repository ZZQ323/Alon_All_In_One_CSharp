namespace TKCompass.Services;

/// <summary>
/// 配置编辑上下文，用于在 ViewModel 和 Dialog 之间传递数据
/// </summary>
public class ConfigEditContext
{
    /// <summary>
    /// 是否为编辑模式（true=编辑，false=新增）
    /// </summary>
    public bool IsEdit { get; set; }

    /// <summary>
    /// 弹窗标题
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// 是否显示代码字段
    /// </summary>
    public bool ShowCodeField { get; set; }

    /// <summary>
    /// 是否显示排序字段
    /// </summary>
    public bool ShowSortOrderField { get; set; }

    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; set; } = "";

    /// <summary>
    /// 标签/名称
    /// </summary>
    public string Label { get; set; } = "";

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int SortOrder { get; set; }
}
