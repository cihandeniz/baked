using Baked.Domain.Inspection;
using Baked.Playground.Ui;
using Baked.Ui;

using static Baked.Ui.Datas;

using B = Baked.Ui.Components;
using C = Baked.Playground.Ui.Components;

namespace Baked.Test.Theme;

public class InspectingComponentAndSchemas : TestSpec
{
    readonly List<DiagnosticMessage> _messages = [];
    readonly InspectTrace _trace = Inspect.TraceHere();
    IDisposable? _diagnostics;

    public override void SetUp()
    {
        base.SetUp();

        _diagnostics = Diagnostics.Start(GiveMe.AString(), result => _messages.AddRange(result.Messages));
    }

    public override void TearDown()
    {
        base.TearDown();

        _diagnostics?.Dispose();
        _messages.Clear();
    }

    [Test]
    public void Allows_inspecting_a_schema_property()
    {
        Inspect.Schema<DataTable.Column>(dtc => dtc.Title);
        var cc = GiveMe.AComponentContext(paths: ["test", "path"]);

        using (_diagnostics)
        {
            _trace.Capture(cc, () => B.DataTableColumn(GiveMe.AString(), options: dtc => dtc.Title = "test title"));
        }

        _messages.Count.ShouldBe(2);
        _messages[0].Level.ShouldBe("info");
        _messages[0].Message.ShouldContain("/test/path");
        _messages[1].Level.ShouldBe("info");
        _messages[1].Message.ShouldContain($"test title");
    }

    [Test]
    public void Allows_inspecting_a_component_property()
    {
        Inspect.Component<Text>(t => t.Prop);
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            _trace.Capture(cc, () => B.Text(options: t => t.Prop = "testProp"));
        }

        _messages.ShouldContain(m => m.Message.Contains("testProp"));
    }

    [Test]
    public void Allows_inspection_on_interfaces()
    {
        Inspect.Component<ISelect>(s => s.OptionLabel);
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            var sb = _trace.Capture(cc, () => B.SelectButton(
                data: Inline(new[] { new { testProp = GiveMe.AString() } }),
                options: sb => sb.OptionLabel = "initialized")
            );
            _trace.Capture(cc, sb, () => sb.Schema.OptionLabel = "updated");
        }

        _messages.ShouldContain(m => m.Message.Contains("initialized"));
        _messages.ShouldContain(m => m.Message.Contains("updated"));
    }

    [Test]
    public void Allows_inspection_on_component_overrides()
    {
        Inspect.Component<MyText>(mt => mt.SomethingExtra);
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            var t = B.Text();
            _trace.Capture(cc, t, () => t.Override(C.MyText(mt => mt.SomethingExtra = "overridden")));
        }

        _messages.ShouldContain(m => m.Message.Contains("overridden"));
    }

    [Test]
    public void Allows_inspecting_a_schema_without_any_property()
    {
        Inspect.Schema<DataTable.Column>();
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            _trace.Capture(cc, () => B.DataTableColumn("test-key"));
        }

        _messages.ShouldContain(m => m.Message.Contains("""
        [gray]<this>:[/] {
          "key": "test-key",
          "component": {
            "type": "Text",
            "schema": {}
          }
        }
        """), customMessage: _messages.Join(", "));
    }

    [Test]
    public void Allows_inspecting_a_component_without_any_property()
    {
        Inspect.Schema<Text>();
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            _trace.Capture(cc, () => B.Text());
        }

        _messages.ShouldContain(m => m.Message.Contains("[gray]<this>:[/] {}"));
    }

    [Test]
    public void Capture_returns_the_expected_schema__so_that_usages_can_return_with_a_single_line()
    {
        Inspect.Schema<DataTable.Column>(dtc => dtc.Title);
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            var dtc = _trace.Capture(cc, () => B.DataTableColumn(GiveMe.AString(), options: t => t.Title = "test title"));

            dtc.Title.ShouldBe("test title");
        }
    }

    [Test]
    public void It_prints_component_path_once_for_consequent_updates()
    {
        Inspect.Schema<DataTable.Column>(dtc => dtc.Title);
        var cc = GiveMe.AComponentContext(paths: ["test", "path"]);

        using (_diagnostics)
        {
            var dtc = _trace.Capture(cc, () => B.DataTableColumn(key: GiveMe.AString(), options: dtc => dtc.Title = "1"));

            _trace.Capture(cc, dtc, () => dtc.Title = "2");
        }

        _messages.Count(c => c.Message.Contains("/test/path")).ShouldBe(1);
    }

    [Test]
    [Ignore("not tested")]
    public void Filters_by_model_context() =>
        this.ShouldFail();

    [Test]
    public void Filters_by_component_context()
    {
        Inspect
            .Where(cc => cc.Path.StartsWith("page-1"))
            .Component<Text>(t => t.Prop)
        ;
        var page1 = GiveMe.AComponentContext(paths: ["page-1"]);
        var page2 = GiveMe.AComponentContext(paths: ["page-2"]);

        using (_diagnostics)
        {
            _trace.Capture(page1, () => B.Text(options: t => t.Prop = "prop1"));
            _trace.Capture(page2, () => B.Text(options: t => t.Prop = "prop2"));
        }

        _messages.ShouldContain(m => m.Message.Contains("prop1"));
        _messages.ShouldNotContain(m => m.Message.Contains("prop2"));
    }

    [Test]
    public void Reports_path_in_gray_for_readability()
    {
        Inspect.Component<Text>();
        var cc = GiveMe.AComponentContext(paths: ["test", "path"]);

        using (_diagnostics)
        {
            _trace.Capture(cc, () => B.Text());
        }

        _messages.ShouldContain(m => m.Message.Contains("[gray]/test/path[/]"));
    }

    [Test]
    public void Reports_schema_type_and_property_name_for_components()
    {
        Inspect.Schema<DataTable.Column>(dtc => dtc.Title);
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            _trace.Capture(cc, () => B.DataTableColumn(GiveMe.AString(), options: dtc => dtc.Title = "test"));
        }

        _messages.ShouldContain(m => m.Message.Contains("<DataTable.Column>"));
        _messages.ShouldContain(m => m.Message.Contains("[gray]Title:[/] test"));
    }

    [Test]
    public void Reports_component_type_and_property_name_for_components()
    {
        Inspect.Component<DataTable>(dt => dt.Paginator);
        var cc = GiveMe.AComponentContext();

        using (_diagnostics)
        {
            _trace.Capture(cc, () => B.DataTable(options: dt => dt.Paginator = true));
        }

        _messages.ShouldContain(m => m.Message.Contains("<DataTable>"));
        _messages.ShouldContain(m => m.Message.Contains("[gray]Paginator:[/] True"));
    }
}