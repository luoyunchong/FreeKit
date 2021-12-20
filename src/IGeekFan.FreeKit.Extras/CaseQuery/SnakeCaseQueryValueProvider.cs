using System.Globalization;
using IGeekFan.FreeKit.Extras.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IGeekFan.FreeKit.Extras.SnakeCaseQuery;

/// <summary>
/// 下划线写法（SnakeCase）
/// </summary>
public class SnakeCaseQueryValueProvider : QueryStringValueProvider
{
    public SnakeCaseQueryValueProvider(
        BindingSource bindingSource,
        IQueryCollection values,
        CultureInfo culture)
        : base(bindingSource, values, culture)
    {
    }

    public override bool ContainsPrefix(string prefix)
    {
        return base.ContainsPrefix(prefix.ToSnakeCase());
    }

    public override ValueProviderResult GetValue(string key)
    {
        return base.GetValue(key.ToSnakeCase());
    }
}


public class LowerCaseQueryValueProvider : QueryStringValueProvider
{
    public LowerCaseQueryValueProvider(
        BindingSource bindingSource,
        IQueryCollection values,
        CultureInfo culture)
        : base(bindingSource, values, culture)
    {
    }

    public override bool ContainsPrefix(string prefix)
    {
        return base.ContainsPrefix(prefix.ToLower());
    }

    public override ValueProviderResult GetValue(string key)
    {
        return base.GetValue(key.ToSnakeCase());
    }
}


public class CamelCaseQueryValueProvider : QueryStringValueProvider
{
    public CamelCaseQueryValueProvider(
        BindingSource bindingSource,
        IQueryCollection values,
        CultureInfo culture)
        : base(bindingSource, values, culture)
    {
    }

    public override bool ContainsPrefix(string prefix)
    {
        return base.ContainsPrefix(prefix.ToCamelCase());
    }

    public override ValueProviderResult GetValue(string key)
    {
        return base.GetValue(key.ToSnakeCase());
    }
}
