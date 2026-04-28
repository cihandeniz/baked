using Baked.Business;
using Baked.CodeGeneration.Diagnostics;
using Baked.Domain.Configuration;
using Baked.Domain.Inspection;
using Baked.Playground.Orm;
using Baked.Theme.Default;
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
        _inspect.Attribute<DataAttribute>(
            attribute: d => d.Label
        );

        var cases = CreateContextCases().OrderBy(c => c.Context.Identifier);
        using (_diagnostics)
        {
            foreach (var (c, _) in cases)
            {
                _trace.Capture(c, () => new DataAttribute("test") { Label = "Test" });
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
        _inspect.Attribute<LabelAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new LabelAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("<this>"));
    }

    [Test]
    [Ignore("not implemented")]
    public void Filters_by_model_context()
    {
        this.ShouldFail();
        // Inspect
        //     .Where(cc => cc.Path.StartsWith("page-1"))
        //     .Component<Text>(t => t.Prop)
        // ;
        // var page1 = GiveMe.AComponentContext(paths: ["page-1"]);
        // var page2 = GiveMe.AComponentContext(paths: ["page-2"]);
        //
        // using (_diagnostics)
        // {
        //     _trace.Capture(page1, () => B.Text(options: t => t.Prop = "prop1"));
        //     _trace.Capture(page2, () => B.Text(options: t => t.Prop = "prop2"));
        // }
        //
        // _messages.ShouldContain(m => m.Message.Contains("prop1"));
        // _messages.ShouldNotContain(m => m.Message.Contains("prop2"));
    }

    [Test]
    public void Reports_member_in_gray_for_readability()
    {
        _inspect.Attribute<LabelAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new LabelAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("[gray]Baked.Playground.Orm.Parent[/]"));
    }

    [Test]
    public void Reports_attribute_type_and_property_name()
    {
        _inspect.Attribute<DataAttribute>(
            attribute: d => d.Label
        );
        var c = new TypeModelContext { Domain = GiveMe.TheDomainModel(), Type = GiveMe.TheTypeModel<Parent>() };

        using (_diagnostics)
        {
            _trace.Capture(c, () => new DataAttribute("test") { Label = "Test" });
        }

        _messages.ShouldContain(m => m.Message.Contains("[[Data]]"));
        _messages.ShouldContain(m => m.Message.Contains("[gray]Label:[/] Test"));
    }

    [Test]
    public void Reports_new_value_as_json_when_value_is_not_value_type_or_string()
    {
        _inspect.Attribute<DataAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new DataAttribute("test-prop") { Label = "Test Label" });
        }

        _messages.ShouldContain(m => m.Message.Contains("""
        {
          "prop": "test-prop",
          "label": "Test Label",
          "visible": true,
          "order": 0
        }
        """));
    }

    [Test]
    public void When_the_inspected_property_of_an_attribute_is_updated__it_reports_only_if_new_value_is_different()
    {
        _inspect.Attribute<DataAttribute>(
            attribute: d => d.Label
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var d = new DataAttribute(GiveMe.AString()) { Label = "initial" };

            _trace.Capture(c, d, () => d.Label = "updated");
            _trace.Capture(c, d, () => d.Label = "updated");
        }

        _messages.Count(c => c.Message.Contains("updated")).ShouldBe(1);
    }

    [Test]
    public void Capture_returns_the_expected_attribute__so_that_usages_can_return_with_a_single_line()
    {
        _inspect.Attribute<DataAttribute>(
            attribute: d => d.Label
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var d = _trace.Capture(c, () => new DataAttribute(GiveMe.AString()) { Label = "test label" });

            d.Label.ShouldBe("test label");
        }
    }

    [Test]
    public void It_prints_member_name_once_for_consequent_updates()
    {
        _inspect.Attribute<DataAttribute>(
            attribute: d => d.Label
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var d = _trace.Capture(c, () => new DataAttribute(GiveMe.AString()) { Label = "1" });
            _trace.Capture(c, d, () => d.Label = "2");
        }

        _messages.Count(c => c.Message.Contains("Parent")).ShouldBe(1);
    }

    [Test]
    public void It_groups_and_sort_messages_by_the_member_id_to_be_reported_together()
    {
        _inspect.Attribute<DataAttribute>(
            attribute: d => d.Label
        );
        var cParent = GiveMe.ATypeModelContext<Parent>();
        var cChild = GiveMe.ATypeModelContext<Child>();

        using (_diagnostics)
        {
            var dParent = _trace.Capture(cParent, () => new DataAttribute(GiveMe.AString()) { Label = "label 3" });
            var dChild = _trace.Capture(cChild, () => new DataAttribute(GiveMe.AString()) { Label = "label 1" });

            _trace.Capture(cParent, dParent, () => dParent.Label = "label 4");
            _trace.Capture(cChild, dChild, () => dChild.Label = "label 2");
        }

        _messages.Count.ShouldBe(6);
        _messages[0].Message.ShouldContain("Child");
        _messages[1].Message.ShouldContain("label 1");
        _messages[2].Message.ShouldContain("label 2");
        _messages[3].Message.ShouldContain("Parent");
        _messages[4].Message.ShouldContain("label 3");
        _messages[5].Message.ShouldContain("label 4");
    }

    [Test]
    public void Capture_does_not_report_when_a_non_inspected_property_is_set_or_updated()
    {
        _inspect.Attribute<DataAttribute>(
            attribute: d => d.Label
        );
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            var d = _trace.Capture(c, () => new DataAttribute(GiveMe.AString()) { Visible = true });
            _trace.Capture(c, d, () => d.Visible = true);
        }

        _messages.Count(c => c.Message.Contains($"{true}")).ShouldBe(0);
        _messages.Count(c => c.Message.Contains($"{false}")).ShouldBe(0);
    }

    [Test]
    public void Captures_and_reports_feature_name_from_stack_trace()
    {
        _inspect.Attribute<LabelAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            new StubFeature(c).Configure(() => new LabelAttribute());
        }

        _messages.ShouldContain(m => Regex.IsMatch(m.Message, @"\[link=.*]StubFeature:\d+\[/]"));
    }

    [Test]
    public void Reports_the_whole_stack_trace_when_feature_is_not_captured()
    {
        _inspect.Attribute<LabelAttribute>();
        var c = GiveMe.ATypeModelContext<Parent>();

        using (_diagnostics)
        {
            _trace.Capture(c, () => new LabelAttribute());
        }

        _messages.ShouldContain(m => m.Message.Contains("[magenta]<unknown>[/]"));
        _messages.ShouldContain(m =>
            Regex.IsMatch(m.Message, @"\[gray].*at Baked[.]Test[.]Domain[.]InspectingAttributes[.][.]ctor\(\).*\[/]",
                RegexOptions.Singleline
            )
        );
    }
}