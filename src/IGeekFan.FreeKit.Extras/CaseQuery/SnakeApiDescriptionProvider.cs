using IGeekFan.FreeKit.Extras.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace IGeekFan.FreeKit.Extras.SnakeCaseQuery;

public class SnakeApiDescriptionProvider : IApiDescriptionProvider
{
    public int Order => 1;

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (ApiParameterDescription parameter in context.Results.SelectMany(x => x.ParameterDescriptions).Where(x => x.Source.Id == "Query" || x.Source.Id == "ModelBinding"))
        {
            parameter.Name = parameter.Name.ToSnakeCase();
        }
    }
}

public class LowerApiDescriptionProvider : IApiDescriptionProvider
{
    public int Order => 1;

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (ApiParameterDescription parameter in context.Results.SelectMany(x => x.ParameterDescriptions).Where(x => x.Source.Id == "Query" || x.Source.Id == "ModelBinding"))
        {
            parameter.Name = parameter.Name.ToLower();
        }
    }
}


public class CamelCaseApiDescriptionProvider : IApiDescriptionProvider
{
    public int Order => 1;

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (ApiParameterDescription parameter in context.Results.SelectMany(x => x.ParameterDescriptions).Where(x => x.Source.Id == "Query" || x.Source.Id == "ModelBinding"))
        {
            parameter.Name = parameter.Name.ToCamelCase();
        }
    }
}


