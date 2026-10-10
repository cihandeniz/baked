using Baked.Architecture;
using Baked.Domain.Configuration;
using Baked.Playground.Caching;
using Baked.Playground.Orm;
using Baked.Playground.Ui;
using Baked.Theme;
using Baked.Theme.Default;
using Baked.Ui;

using static Baked.Theme.Default.DomainDatas;

using B = Baked.Ui.Components;
using C = Baked.Playground.Ui.Components;
using Route = Baked.Theme.Route;

namespace Baked.Playground.Theme.Custom;

public class CustomThemeFeature(IEnumerable<Func<Router, Route>> routes)
    : DefaultThemeFeature(routes.Select(r => r(new())),
        _sideMenuOptions: sm => sm.Footer = B.LanguageSwitcher(),
        _errorPageOptions: ep =>
        {
            ep.ErrorInfos[503] = new(
                Title: "Service Unavailable",
                Message: "The service is currently unavailable. Please try again later."
            )
            { CustomMessage = true };
        }
    )
{
    public override void Configure(LayerConfigurator configurator)
    {
        base.Configure(configurator);

        configurator.Domain.ConfigureConventions(conventions =>
        {
            // Custom theme CSV formatter settings
            conventions.EditMethodSchema<DataTable.Export>(
                schema: (dte, _, cc) =>
                {
                    var (_, l) = cc;

                    dte.ButtonLabel = l("Export as CSV");
                    dte.Formatter = "useCsvFormatter";
                    dte.AppendParameters = true;
                    dte.ParameterSeparator = "_";
                    dte.ParameterFormatter = "useLocaleParameterFormatter";
                }
            );

            // String api rendering
            conventions.AddMethodComponent(
                when: c => c.Method.DefaultOverload.ReturnType.Is<string>(),
                where: cc => cc.Path.EndsWith("data-panel", "content"),
                component: () => B.Text()
            );
            conventions.EditMethodComponent<Text>(
                when: c => c.Method.DefaultOverload.ReturnType.Is<string>(),
                component: (t, c, cc) => t.Data = c.Method.GenerateSchema<RemoteData>(cc.Drill("data")),
                order: Order.At.Min
            );
            conventions.EditMethodComponent<Text>(
                component: t => t.Override(C.MyText())
            );
            conventions.EditMethodComponent<Text>(
                component: t => t.Schema.MaxLength = 100
            );
            conventions.EditMethodComponent<Text>(
                component: t =>
                {
                    if (t.Schema is not MyText mt) { return; }

                    mt.SomethingExtra = "this is extra!";
                }
            );

            // Non-localized enums
            conventions.AddTypeSchema(
                schema: (c, cc) => EnumInline(c.Type, cc, requireLocalization: false),
                when: c => c.Type.Is<CacheKey>() || c.Type.Is<RowCount>()
            );

            // Custom routes
            conventions.SetTypeRoute<Entity>("/entities/[id]");
            conventions.SetTypeRoute<Parent>("/parents/[id]");
            conventions.SetMethodRoute<FormSample>(nameof(FormSample.NewParent), "/form-sample/parents/new");
        });

        configurator.Ui.ConfigureComponentExports(c =>
        {
            c.AddFromExtensions(typeof(C));
        });

        configurator.Ui.ConfigurePageDescriptors(pages =>
        {
            pages.AddPage(new LoginPage { Path = "login", Layout = "modal" });
            pages.AddPage(new RoutedPage { Path = "page/with/route/pageWithRoute", Layout = "default" });
            pages.AddPage(new RoutedPage { Path = "first/[id]", Layout = "default" });
            pages.AddPage(new RoutedPage { Path = "first/[firstId]/second/[secondId]", Layout = "default" });
        });
    }
}