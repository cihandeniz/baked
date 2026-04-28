using Baked.CodeGeneration.Diagnostics;
using Baked.Domain.Configuration;
using Baked.Domain.Inspection;
using Baked.Playground.Business;
using Baked.Playground.Orm;
using System.Text.RegularExpressions;

namespace Baked.Test.Domain;

public class InspectingAttributes : TestSpec
{
    readonly List<DiagnosticMessage> _messages = [];
    readonly Trace _trace = Trace.Here();
    Inspect _inspect = default!;
    IDisposable? _diagnostics;

    public override void SetUp()
    {
        base.SetUp();

        _inspect = new();
        _diagnostics = Diagnostics.Start(GiveMe.AString(), result => _messages.AddRange(result.Messages));
    }

    public override void TearDown()
    {
        base.TearDown();

        _diagnostics?.Dispose();
        _messages.Clear();
    }

    IEnumerable<(DomainModelContext Context, string MemberName)> CreateContextCases()
    {
        var domain = GiveMe.TheDomainModel();
        var parent = GiveMe.TheTypeModel<Parent>().GetMembers();
        var name = parent.Properties[nameof(Parent.Name)];
        var addChild = parent.Methods[nameof(Parent.AddChild)];
        var pName = addChild.DefaultOverload.Parameters["name"];

        yield return (new TypeModelMetadataContext { Domain = domain, Type = parent }, "Baked.Playground.Orm.Parent");
        yield return (new TypeModelMembersContext { Domain = domain, Type = parent }, "Baked.Playground.Orm.Parent");
        yield return
            (
                new PropertyModelContext
                {
                    Domain = domain,
                    Type = parent,
                    Property = name
                },
                $"Baked.Playground.Orm.Parent.{nameof(Parent.Name)}"
            );
        yield return
            (
                new MethodModelContext
                {
                    Domain = domain,
                    Type = parent,
                    Method = addChild
                },
                $"Baked.Playground.Orm.Parent.{nameof(Parent.AddChild)}"
            );
        yield return
            (
                new ParameterModelContext
                {
                    Domain = domain,
                    Type = parent,
                    Method = addChild,
                    MethodOverload = addChild.DefaultOverload,
                    Parameter = pName
                },
                $"Baked.Playground.Orm.Parent.{nameof(Parent.AddChild)}.name"
            );
    }

    [Test]
    public void When_an_attribute_is_added_with_a_non_null_on_the_inspected_property__it_reports_applied_member_and_the_initial_value()
    {
        _inspect.Attribute<CustomAttribute>(
            attribute: ca => ca.Value
        );

        var cases = CreateContextCases().OrderBy(ca => ca.Context.Identifier);
        using (_diagnostics)
        {
            foreach (var (c, _) in cases)
            {
                _trace.Capture(c, () => new CustomAttribute { Value = "Test" });
            }
        }

        _messages.Count.ShouldBe(2 * cases.Count());
        var i = 0;
        foreach (var (_, memberName) in cases)
        {
            _messages[0 + i * 2].Level.ShouldBe("info");
            _messages[0 + i * 2].Message.ShouldContain(memberName);
            _messages[1 + i * 2].Level.ShouldBe("info");
            _messages[1 + i * 2].Message.ShouldContain($"Test");

            i++;
        }
    }

    [Test]
    public void Allows_inspecting_an_attribute_without_any_property()
    {
        _inspect.Attribute<CustomAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new CustomAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("<this>"));
    }

    [Test]
    public void Provides_when_filter_to_filter_by_type_model_context()
    {
        _inspect.TypeAttribute<CustomAttribute>(
            when: ca => ca.Type.Is<Parent>()
        );
        var domain = GiveMe.TheDomainModel();
        var cParent = new TypeModelMetadataContext { Domain = domain, Type = domain.Types[typeof(Parent)].GetMetadata() };
        var cChild = new TypeModelMetadataContext { Domain = domain, Type = domain.Types[typeof(Child)].GetMetadata() };

        using (_diagnostics)
        {
            _trace.Capture(cParent, () => new CustomAttribute());
            _trace.Capture(cChild, () => new CustomAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("Baked.Playground.Orm.Parent"));
        _messages.ShouldNotContain(m => m.Message.Contains("Baked.Playground.Orm.Child"));
    }

    [Test]
    public void Provides_when_filter_to_filter_by_property_model_context()
    {
        _inspect.PropertyAttribute<CustomAttribute>(
            when: ca => ca.Property.Name == nameof(Parent.Id)
        );
        var domain = GiveMe.TheDomainModel();
        var parent = domain.Types[typeof(Parent)].GetMembers();
        var cId = new PropertyModelContext { Domain = domain, Type = parent, Property = parent.Properties[nameof(Parent.Id)] };
        var cName = new PropertyModelContext { Domain = domain, Type = parent, Property = parent.Properties[nameof(Parent.Name)] };

        using (_diagnostics)
        {
            _trace.Capture(cId, () => new CustomAttribute());
            _trace.Capture(cName, () => new CustomAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("Baked.Playground.Orm.Parent.Id"));
        _messages.ShouldNotContain(m => m.Message.Contains("Baked.Playground.Orm.Parent.Name"));
    }

    [Test]
    public void Provides_when_filter_to_filter_by_method_model_context()
    {
        _inspect.MethodAttribute<CustomAttribute>(
            when: ca => ca.Method.Name == nameof(Parent.AddChild)
        );
        var domain = GiveMe.TheDomainModel();
        var parent = domain.Types[typeof(Parent)].GetMembers();
        var cAddChild = new MethodModelContext { Domain = domain, Type = parent, Method = parent.Methods[nameof(Parent.AddChild)] };
        var cGetChildren = new MethodModelContext { Domain = domain, Type = parent, Method = parent.Methods[nameof(Parent.GetChildren)] };

        using (_diagnostics)
        {
            _trace.Capture(cAddChild, () => new CustomAttribute());
            _trace.Capture(cGetChildren, () => new CustomAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("Baked.Playground.Orm.Parent.AddChild"));
        _messages.ShouldNotContain(m => m.Message.Contains("Baked.Playground.Orm.Parent.GetChildren"));
    }

    [Test]
    public void Provides_when_filter_to_filter_by_parameter_model_context()
    {
        _inspect.ParameterAttribute<CustomAttribute>(
            when: ca => ca.Parameter.Name == "name"
        );
        var domain = GiveMe.TheDomainModel();
        var parent = domain.Types[typeof(Parent)].GetMembers();
        var update = parent.Methods[nameof(Parent.Update)];
        var cName = new ParameterModelContext
        {
            Domain = domain,
            Type = parent,
            Method = update,
            MethodOverload = update.DefaultOverload,
            Parameter = update.DefaultOverload.Parameters["name"]
        };
        var cSurname = new ParameterModelContext
        {
            Domain = domain,
            Type = parent,
            Method = parent.Methods[nameof(Parent.Update)],
            MethodOverload = update.DefaultOverload,
            Parameter = update.DefaultOverload.Parameters["surname"]
        };

        using (_diagnostics)
        {
            _trace.Capture(cName, () => new CustomAttribute());
            _trace.Capture(cSurname, () => new CustomAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("Baked.Playground.Orm.Parent.Update.name"));
        _messages.ShouldNotContain(m => m.Message.Contains("Baked.Playground.Orm.Parent.Update.surname"));
    }

    [Test]
    public void Reports_member_in_gray_for_readability()
    {
        _inspect.Attribute<CustomAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new CustomAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("[gray]Baked.Playground.Orm.Parent[/]"));
    }

    [Test]
    public void Reports_attribute_type_and_property_name()
    {
        _inspect.Attribute<CustomAttribute>(
            attribute: d => d.Value
        );
        var c = new TypeModelContext { Domain = GiveMe.TheDomainModel(), Type = GiveMe.TheTypeModel<Parent>() };

        using (_diagnostics)
        {
            _trace.Capture(c, () => new CustomAttribute { Value = "Test" });
        }

        _messages.ShouldContain(m => m.Message.Contains("[[Custom]]"));
        _messages.ShouldContain(m => m.Message.Contains("[gray]Value:[/] Test"));
    }

    [Test]
    public void Reports_new_value_as_json_when_value_is_not_value_type_or_string()
    {
        _inspect.Attribute<CustomAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new CustomAttribute { Value = "Test" });
        }

        _messages.ShouldContain(m => m.Message.Contains("""
        {
          "value": "Test"
        }
        """));
    }

    [Test]
    public void When_the_inspected_property_of_an_attribute_is_updated__it_reports_only_if_new_value_is_different()
    {
        _inspect.Attribute<CustomAttribute>(
            attribute: ca => ca.Value
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var ca = new CustomAttribute { Value = "initial" };

            _trace.Capture(c, ca, () => ca.Value = "updated");
            _trace.Capture(c, ca, () => ca.Value = "updated");
        }

        _messages.Count(ca => ca.Message.Contains("updated")).ShouldBe(1);
    }

    [Test]
    public void Capture_returns_the_expected_attribute__so_that_usages_can_return_with_a_single_line()
    {
        _inspect.Attribute<CustomAttribute>(
            attribute: ca => ca.Value
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var ca = _trace.Capture(c, () => new CustomAttribute { Value = "test" });

            ca.Value.ShouldBe("test");
        }
    }

    [Test]
    public void It_prints_member_name_once_for_consequent_updates()
    {
        _inspect.Attribute<CustomAttribute>(
            attribute: ca => ca.Value
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var ca = _trace.Capture(c, () => new CustomAttribute { Value = "1" });
            _trace.Capture(c, ca, () => ca.Value = "2");
        }

        _messages.Count(ca => ca.Message.Contains("Parent")).ShouldBe(1);
    }

    [Test]
    public void It_groups_and_sort_messages_by_the_member_id_to_be_reported_together()
    {
        _inspect.Attribute<CustomAttribute>(
            attribute: ca => ca.Value
        );
        var cParent = GiveMe.ATypeModelContext<Parent>();
        var cChild = GiveMe.ATypeModelContext<Child>();

        using (_diagnostics)
        {
            var dParent = _trace.Capture(cParent, () => new CustomAttribute { Value = "value 3" });
            var dChild = _trace.Capture(cChild, () => new CustomAttribute { Value = "value 1" });

            _trace.Capture(cParent, dParent, () => dParent.Value = "value 4");
            _trace.Capture(cChild, dChild, () => dChild.Value = "value 2");
        }

        _messages.Count.ShouldBe(6);
        _messages[0].Message.ShouldContain("Child");
        _messages[1].Message.ShouldContain("value 1");
        _messages[2].Message.ShouldContain("value 2");
        _messages[3].Message.ShouldContain("Parent");
        _messages[4].Message.ShouldContain("value 3");
        _messages[5].Message.ShouldContain("value 4");
    }

    [Test]
    public void Capture_does_not_report_when_a_non_inspected_property_is_set_or_updated()
    {
        _inspect.Attribute<CustomAttribute>(
            attribute: ca => ca.Value
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var ca = _trace.Capture(c, () => new CustomAttribute { SecondValue = "create" });
            _trace.Capture(c, ca, () => ca.SecondValue = "update");
        }

        _messages.Count(m => m.Message.Contains($"init")).ShouldBe(0);
        _messages.Count(m => m.Message.Contains($"create")).ShouldBe(0);
    }

    [Test]
    public void Captures_and_reports_feature_name_from_stack_trace()
    {
        _inspect.Attribute<CustomAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            new StubFeature(c).Configure(() => new CustomAttribute());
        }

        _messages.ShouldContain(m => Regex.IsMatch(m.Message, @"\[link=.*]StubFeature:\d+\[/]"));
    }

    [Test]
    public void Reports_the_whole_stack_trace_when_feature_is_not_captured()
    {
        _inspect.Attribute<CustomAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new CustomAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("[magenta]<unknown>[/]"));
        _messages.ShouldContain(m =>
            Regex.IsMatch(m.Message, @"\[gray].*at Baked[.]Test[.]Domain[.]InspectingAttributes[.][.]ctor\(\).*\[/]",
                RegexOptions.Singleline
            )
        );
    }
}