namespace TKCompass.Domain.Filters;

/// <summary>
/// 筛选操作符枚举
/// </summary>
public enum FilterOperator
{
    Equals,
    NotEquals,
    Contains,
    NotContains,
    StartsWith,
    EndsWith,
    In,
    NotIn,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    DateRange,
    IsNull,
    IsNotNull
}
