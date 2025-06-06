﻿using Baked.Architecture;
using Baked.RestApi.Model;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Baked.CodingStyle.UseNullableTypes;

public class UseNullableTypesCodingStyleFeature : IFeature<CodingStyleConfigurator>
{
    NullabilityInfoContext _nullability = new();

    public void Configure(LayerConfigurator configurator)
    {
        configurator.ConfigureDomainModelBuilder(builder =>
        {
            builder.Conventions.AddTypeMetadata(new ApiInputAttribute(),
                when: c =>
                    c.Type.IsAssignableTo(typeof(Nullable<>)) &&
                    c.Type.GenericTypeArguments.FirstOrDefault()?.Model.TryGetMetadata(out var genericArgumentMetadata) == true &&
                    genericArgumentMetadata.Has<ApiInputAttribute>(),
                order: int.MinValue
            );
            builder.Conventions.AddParameterMetadata(new NotNullAttribute(),
                when: c =>
                {
                    var nullable = false;
                    c.Parameter.Apply(p =>
                    {
                        nullable = _nullability.Create(p).WriteState is NullabilityState.Nullable;
                    });

                    return !nullable;
                }
            );

            builder.Conventions.Add(new NonOptionalNotNullParametersAreRequiredConvention());
            builder.Conventions.Add(new SetDefaultValueForNullableEnumConvention());
        });

        configurator.ConfigureApiModel(api =>
        {
            api.Usings.AddRange([
                "Microsoft.AspNetCore.Mvc.ModelBinding",
                "Newtonsoft.Json",
                "System.ComponentModel.DataAnnotations"
            ]);
        });
    }
}