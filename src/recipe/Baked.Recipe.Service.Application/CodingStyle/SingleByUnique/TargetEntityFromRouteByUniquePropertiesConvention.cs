﻿using Baked.Business;
using Baked.Domain.Configuration;
using Baked.RestApi.Model;
using Humanizer;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Baked.CodingStyle.SingleByUnique;

public class TargetEntityFromRouteByUniquePropertiesConvention : IDomainModelConvention<MethodModelContext>
{
    protected virtual bool TryGetEntityType(MethodModelContext context, [NotNullWhen(true)] out Domain.Model.TypeModel? entityType, out Domain.Model.TypeModel? castTo)
    {
        entityType = context.Type;
        castTo = null;

        return true;
    }

    public void Apply(MethodModelContext context)
    {
        if (!context.Method.TryGetSingle<ActionModelAttribute>(out var action)) { return; }
        if (!action.Parameter.TryGetValue(ParameterModelAttribute.TargetParameterName, out var parameter)) { return; }
        if (context.Method.Has<InitializerAttribute>()) { return; }

        if (!TryGetEntityType(context, out var entityType, out var castTo)) { return; }
        if (!entityType.TryGetQueryType(context.Domain, out var queryType)) { return; }
        if (!queryType.TryGetMembers(out var queryMembers)) { return; }

        var singleByUniques = queryMembers.Methods.Having<SingleByUniqueAttribute>();
        if (!singleByUniques.Any()) { return; }

        var uniques = singleByUniques.Select(sbu => sbu.GetSingle<SingleByUniqueAttribute>());
        parameter.Type = "string";
        parameter.Name = $"idOr{uniques.Select(u => u.PropertyName).Join("Or")}";
        action.RouteParts = action.RouteParts.Replace("{id:guid}", $"{{{parameter.Name}}}");

        var findTargetStatements = new StringBuilder();
        findTargetStatements.Append($$"""
            {{context.Type.CSharpFriendlyFullName}} __foundTarget = null;
            if(Guid.TryParse({{parameter.Name}}, out var id))
            {
                __foundTarget = {{action.FindTargetStatement}};
            }
        """);

        var queryParameter = action.AddAsService(queryType);
        SingleByUniqueAttribute? fallback = null;
        foreach (var singleByUnique in singleByUniques)
        {
            var unique = singleByUnique.GetSingle<SingleByUniqueAttribute>();
            var uniqueParameter = singleByUnique.DefaultOverload.Parameters[unique.PropertyName.Camelize()];
            if (uniqueParameter.ParameterType.IsEnum)
            {
                findTargetStatements.Append($$"""
                    else if(Enum.TryParse<{{uniqueParameter.ParameterType.CSharpFriendlyFullName}}>({{parameter.Name}}, true, out var @{{uniqueParameter.Name}}))
                    {
                        __foundTarget = {{queryParameter.BuildSingleBy($"@{uniqueParameter.Name}", property: unique.PropertyName, fromRoute: true, castTo: castTo)}};
                    }
                """);
            }
            else if (uniqueParameter.ParameterType.Is<string>())
            {
                fallback = unique;
            }
        }

        if (fallback is not null)
        {
            findTargetStatements.Append($$"""
                else
                {
                    __foundTarget = {{queryParameter.BuildSingleBy(parameter.Name, property: fallback.PropertyName, fromRoute: true, castTo: castTo)}};
                }
            """);
        }
        else
        {
            findTargetStatements.Append($$"""
                else
                {
                    throw new {{nameof(RouteParameterIsNotValidException)}}("{{parameter.Name}}", {{parameter.Name}});
                }
            """);
        }

        action.PreparationStatements.Add(findTargetStatements.ToString());
        action.FindTargetStatement = "__foundTarget";
    }
}