﻿using System.Reflection;

namespace Do.Domain.Model;

public class DomainModelBuilder(DomainBuilderOptions _domainBuilderOptions)
{
    readonly Dictionary<string, AssemblyModel> _assemblies = [];
    readonly Dictionary<string, TypeModel> _types = [];

    public DomainModel BuildFrom(IAssemblyCollection assemblyCollection, ITypeCollection typeCollection)
    {
        foreach (var assemblyDescriptor in assemblyCollection)
        {
            var model = new AssemblyModel(assemblyDescriptor.Assembly);
            _assemblies.Add(model.Id, model);
        }

        foreach (var typeDescriptor in typeCollection)
        {
            if (_types.ContainsKey(TypeModel.GetId(typeDescriptor.Type))) { continue; }

            var model = new TypeModel(typeDescriptor.Type);
            _types.Add(model.Id, model);

            BuildTypeModel(model, typeDescriptor.Type);
        }

        return new(new(_assemblies.Values), new(_types.Values));
    }

    void BuildTypeModel(TypeModel typeModel, Type type)
    {
        if (_domainBuilderOptions.TypeIsBuiltConventions.Any(c => !c(typeModel))) { return; }

        typeModel.Init(
            genericTypeArguments: new(BuildGenericTypeArguments(type)),
            customAttributes: new(BuildCustomAttributes(type)),
            properties: new(BuildProperties(type)),
            methods: new(BuildMethods(type))
        );
    }

    TypeModel GetOrCreateTypeModel(Type type)
    {
        if (_types.TryGetValue(TypeModel.GetId(type), out var result)) { return result; }

        var newTypeModel = new TypeModel(type);
        _types[newTypeModel.Id] = newTypeModel;

        BuildTypeModel(newTypeModel, type);

        return newTypeModel;
    }

    List<MethodModel> BuildMethods(Type type)
    {
        var result = new Dictionary<string, MethodModel>();

        var constructorInfos = type.GetConstructors(_domainBuilderOptions.ConstuctorBindingFlags) ?? [];

        result[".ctor"] = new(".ctor", true, new(constructorInfos.Select(BuildConstructorOverload).ToList()));

        var methodInfos = type.GetMethods(_domainBuilderOptions.MethodBindingFlags) ?? [];
        foreach (var group in methodInfos.GroupBy(m => m.Name))
        {
            result[group.Key] = new(group.Key, false, new(group.Select(BuildMethodOverload).ToList()));
        }

        return [.. result.Values];
    }

    IEnumerable<AttributeModel> BuildCustomAttributes(MemberInfo member) =>
        member.CustomAttributes.Select(attr => new AttributeModel(GetOrCreateTypeModel(attr.AttributeType)));

    IEnumerable<TypeModel> BuildGenericTypeArguments(Type type) =>
        type.GenericTypeArguments.Select(GetOrCreateTypeModel);

    OverloadModel BuildConstructorOverload(ConstructorInfo constructor) =>
        new(constructor.IsPublic, constructor.IsFamily, constructor.IsVirtual, new(BuildParameters(constructor)), new(BuildCustomAttributes(constructor)));

    OverloadModel BuildMethodOverload(MethodInfo method) =>
        new(method.IsPublic, method.IsFamily, method.IsVirtual, new(BuildParameters(method)), new(BuildCustomAttributes(method)), GetOrCreateTypeModel(method.ReturnType));

    IEnumerable<ParameterModel> BuildParameters(MethodBase method) =>
        method.GetParameters()
            .Select(p => new ParameterModel(p.Name ?? string.Empty, GetOrCreateTypeModel(p.ParameterType), p.IsOptional, p.DefaultValue));

    IEnumerable<PropertyModel> BuildProperties(Type type) =>
        type.GetProperties(_domainBuilderOptions.PropertyBindingFlags).Select(BuildProperty);

    PropertyModel BuildProperty(PropertyInfo property) =>
        new(property.Name, GetOrCreateTypeModel(property.PropertyType), IsPublic(property), IsVirtual(property));

    static bool IsPublic(PropertyInfo source) =>
        source.GetMethod?.IsPublic == true;

    static bool IsVirtual(PropertyInfo source) =>
        source.GetMethod?.IsVirtual == true;
}


