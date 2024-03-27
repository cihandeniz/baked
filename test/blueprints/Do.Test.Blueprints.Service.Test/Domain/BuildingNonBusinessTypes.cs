﻿using Do.Domain.Model;
using Do.Orm;

namespace Do.Test.Domain;

public class BuildingNonBusinessTypes : TestServiceSpec
{
    [Test]
    public void Non_business_types_are_added_to_type_collection([Values(typeof(string), typeof(int), typeof(Task), typeof(Func<Entity>), typeof(IQueryContext<Entity>))] Type type)
    {
        DomainModel.ReferencedTypes.Contains(type).ShouldBeTrue();
    }

    [Test]
    public void Non_business_types_with_no_generic_parameters_are_initialized_with_empty_collections([Values(typeof(string), typeof(int), typeof(Task))] Type type)
    {
        var model = DomainModel.ReferencedTypes[type];

        model.Properties.ShouldNotBeNull();
        model.Properties.Count().ShouldBe(0);

        model.GenericTypeArguments.ShouldNotBeNull();
        model.GenericTypeArguments.Count().ShouldBe(0);

        model.Methods.ShouldNotBeNull();
        model.Methods.Length.ShouldBe(0);

        model.Interfaces.ShouldNotBeNull();
        model.Interfaces.Count().ShouldBe(0);

        model.CustomAttributes.ShouldNotBeNull();
        model.CustomAttributes.Count().ShouldBe(0);

        model.Constructors.ShouldBeNull();
        model.Constructors.Length.ShouldBe(0);
    }

    [Test]
    public void Non_business_types_with_generic_parameters_are_initialized_with_generic_arguments([Values(typeof(List<Entity>), typeof(Func<Entity>), typeof(IQueryContext<Entity>))] Type type)
    {
        var entityModel = DomainModel.ReferencedTypes[typeof(Entity)];
        var listEntity = DomainModel.ReferencedTypes[type];

        listEntity.ShouldNotBeNull();
        listEntity.GenericTypeArguments.Count().ShouldBe(1);
        listEntity.GenericTypeArguments.First().ShouldBe(entityModel);
    }

    [Test]
    public void Base_type_is_added_for_task()
    {
        var model = DomainModel.ReferencedTypes[typeof(Task<TransientWithTask>)];

        model.ShouldNotBeNull();
        model.BaseType.ShouldNotBeNull();
        model.BaseType.IsAssignableTo<Task>();
    }
}
