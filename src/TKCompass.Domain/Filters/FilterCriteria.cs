namespace TKCompass.Domain.Filters;

/// <summary>
/// 筛选条件类（支持嵌套和组合）
/// </summary>
public class FilterCriteria
{
    /// <summary>
    /// 字段名（如 "Handle", "CooperationStatus"）
    /// </summary>
    public string FieldName { get; set; } = string.Empty;
    
    /// <summary>
    /// 筛选操作符
    /// </summary>
    public FilterOperator Operator { get; set; }
    
    /// <summary>
    /// 筛选值（支持多值，如 In/NotIn）
    /// </summary>
    public List<object?> Values { get; set; } = new();
    
    /// <summary>
    /// 嵌套条件（用于组合 And/Or）
    /// </summary>
    public List<FilterCriteria> Conditions { get; set; } = new();
    
    /// <summary>
    /// 逻辑操作符（And/Or），仅当有嵌套条件时使用
    /// </summary>
    public FilterLogicOperator LogicOperator { get; set; } = FilterLogicOperator.And;
    
    /// <summary>
    /// 是否为组合条件（包含嵌套条件）
    /// </summary>
    public bool IsComposite => Conditions.Count > 0;
    
    /// <summary>
    /// 验证条件有效性
    /// </summary>
    public bool IsValid()
    {
        if (IsComposite)
        {
            return Conditions.All(c => c.IsValid());
        }
        
        return !string.IsNullOrWhiteSpace(FieldName) && 
               (Operator == FilterOperator.IsNull || 
                Operator == FilterOperator.IsNotNull || 
                Values.Count > 0);
    }
    
    /// <summary>
    /// 创建单条件筛选
    /// </summary>
    public static FilterCriteria Create(string fieldName, FilterOperator op, object? value = null)
    {
        var criteria = new FilterCriteria
        {
            FieldName = fieldName,
            Operator = op
        };
        
        if (value != null)
        {
            criteria.Values.Add(value);
        }
        
        return criteria;
    }
    
    /// <summary>
    /// 创建多值筛选（In/NotIn）
    /// </summary>
    public static FilterCriteria Create(string fieldName, FilterOperator op, IEnumerable<object?> values)
    {
        return new FilterCriteria
        {
            FieldName = fieldName,
            Operator = op,
            Values = values.ToList()
        };
    }
    
    /// <summary>
    /// 创建组合条件
    /// </summary>
    public static FilterCriteria Combine(FilterLogicOperator logic, params FilterCriteria[] conditions)
    {
        return new FilterCriteria
        {
            LogicOperator = logic,
            Conditions = conditions.ToList()
        };
    }
}
