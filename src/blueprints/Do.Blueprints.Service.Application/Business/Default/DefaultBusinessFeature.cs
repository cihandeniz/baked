﻿using Do.Architecture;
using Do.Domain.Configuration;
using Do.Domain.Model;
using Do.RestApi.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Do.Business.Default;

public class DefaultBusinessFeature(List<Assembly> _domainAssemblies)
    : IFeature<BusinessConfigurator>
{
    const BindingFlags _defaultMemberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    public void Configure(LayerConfigurator configurator)
    {
        configurator.ConfigureDomainAssemblyCollection(assemblies =>
        {
            foreach (var assembly in _domainAssemblies)
            {
                assemblies.Add(assembly);
            }
        });

        configurator.ConfigureDomainBuilderOptions(options =>
        {
            options.ConstuctorBindingFlags = _defaultMemberBindingFlags;
            options.MethodBindingFlags = _defaultMemberBindingFlags;
            options.PropertyBindingFlags = _defaultMemberBindingFlags;

            options.Indexers.Add(new AttributeIndexer<TransientAttribute>());
            options.Indexers.Add(new AttributeIndexer<ScopedAttribute>());
            options.Indexers.Add(new AttributeIndexer<SingletonAttribute>());
            options.Indexers.Add(new AttributeIndexer<PublicServiceAttribute>());
        });

        configurator.ConfigureDomainMetaData(metadata =>
        {
            metadata
                .Type
                    .Add(
                        add: new DataClassAttribute(),
                        when: type => type.Methods.Contains("<Clone>$"), // if type is record
                        order: int.MinValue
                    )
                    .Add(
                        add: new TransientAttribute(),
                        when: type => !type.IsIgnored() && type.Methods.TryGetValue("With", out var with) && with.CanReturn(type),
                        order: 10
                    )
                    .Add(
                        add: new ScopedAttribute(),
                        when: type => !type.IsIgnored() && type.IsAssignableTo<IScoped>(),
                        order: 20
                    )
                    .Add(
                        add: new SingletonAttribute(),
                        when: type => !type.IsIgnored() && !type.HasAttribute<TransientAttribute>() && !type.HasAttribute<ScopedAttribute>() && type.Properties.All(p => !p.IsPublic),
                        order: 30
                    );

            metadata
                .Method
                    .Add(
                        add: (model, adder) => adder.Add(model, new PublicServiceAttribute()),
                        when: method => !method.IsConstructor && method.Overloads.Any(o => o.IsPublic),
                        order: int.MinValue
                    );
        });

        configurator.ConfigureServiceCollection(services =>
        {
            var domainModel = configurator.Context.GetDomainModel();
            foreach (var type in domainModel.Types.WithAttribute<TransientAttribute>())
            {
                type.Apply(t =>
                {
                    services.AddTransientWithFactory(t);
                    type.Interfaces
                        .Where(i => i.IsBusinessType)
                        .Apply(i => services.AddTransientWithFactory(i, t));
                });
            }

            foreach (var type in domainModel.Types.WithAttribute<ScopedAttribute>())
            {
                type.Apply(t =>
                {
                    services.AddScopedWithFactory(t);
                    type.Interfaces
                        .Where(i => i.IsBusinessType)
                        .Apply(i => services.AddScopedWithFactory(i, t));
                });
            }

            foreach (var type in domainModel.Types.WithAttribute<SingletonAttribute>())
            {
                type.Apply(t =>
                {
                    services.AddSingleton(t);
                    type.Interfaces
                        .Where(i => i.IsBusinessType)
                        .Apply(i => services.AddSingleton(i, t, forward: true));
                });
            }
        });

        configurator.ConfigureApiModel(api =>
        {
            api.References.AddRange(_domainAssemblies);

            var domainModel = configurator.Context.GetDomainModel();
            foreach (var type in domainModel.Types.Where(t => !t.IsIgnored()))
            {
                if (type.FullName is null) { continue; }
                if (!type.HasAttribute<SingletonAttribute>()) { continue; } // TODO for now only singleton

                var controller = new ControllerModel(type.Name);
                foreach (var method in type.Methods.WithAttribute<PublicServiceAttribute>())
                {
                    var overload = method.Overloads.OrderByDescending(o => o.Parameters.Count).First();
                    if (overload.ReturnType is null) { continue; }

                    if (overload.Parameters.Count > 0) { continue; } // TODO for now only parameterless
                    if (!overload.ReturnType.IsAssignableTo(typeof(void)) &&
                        !overload.ReturnType.IsAssignableTo(typeof(Task))) { continue; } // TODO for now only void

                    controller.Actions.Add(
                        new(
                            Name: method.Name,
                            Method: HttpMethod.Post,
                            Route: $"generated/{type.Name}/{method.Name}",
                            Return: new(async: overload.ReturnType.IsAssignableTo(typeof(Task))),
                            Statements: new(
                                FindTarget: "target",
                                InvokeMethod: new(method.Name)
                            )
                        )
                        { Parameters = [new(ParameterModelFrom.Services, type.FullName, "target")] }
                    );
                }

                api.Controllers.Add(controller);
            }
        });

        configurator.ConfigureMvcNewtonsoftJsonOptions(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        });
    }
}
