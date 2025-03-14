﻿using Baked.Architecture;
using Baked.ExceptionHandling;
using Baked.RestApi.Model;
using Baked.Test.Authentication;
using Baked.Test.Business;
using Baked.Test.CodingStyle.RichTransient;
using Baked.Test.Core;
using Baked.Test.ExceptionHandling;
using Baked.Test.Orm;
using Baked.Theme.Admin;
using Baked.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using static Baked.Theme.Admin.Components;
using static Baked.Ui.Datas;

namespace Baked.Test.ConfigurationOverrider;

public class ConfigurationOverriderFeature : IFeature
{
    public void Configure(LayerConfigurator configurator)
    {
        configurator.ConfigureDomainModelBuilder(builder =>
        {
            builder.Conventions.AddSingleById<Entities>();
            builder.Conventions.AddSingleById<Parents>();
            builder.Conventions.AddSingleById<Children>();
            builder.Conventions.AddConfigureAction<AuthenticationSamples>(nameof(AuthenticationSamples.FormPostAuthenticate), useForm: true);
            builder.Conventions.AddConfigureAction<DocumentationSamples>(nameof(DocumentationSamples.Route), parameter: p =>
            {
                p["route"].From = ParameterModelFrom.Route;
                p["route"].RoutePosition = 2;
            });
            builder.Conventions.AddConfigureAction<ExceptionSamples>(nameof(ExceptionSamples.Throw), parameter: p => p["handled"].From = ParameterModelFrom.Query);

            builder.Conventions.AddOverrideAction<OverrideSamples>(nameof(OverrideSamples.UpdateRoute),
                routeParts: ["override-samples", "override", "update-route"],
                method: HttpMethod.Post
            );
            builder.Conventions.AddOverrideAction<OverrideSamples>(nameof(OverrideSamples.Parameter),
                parameter: parameter =>
                {
                    parameter["parameter"].Name = "id";
                    parameter["parameter"].From = ParameterModelFrom.Route;
                    parameter["parameter"].RoutePosition = 2;
                }
            );
            builder.Conventions.AddOverrideAction<OverrideSamples>(nameof(OverrideSamples.RequestClass),
                useRequestClassForBody: false
            );
        });

        configurator.ConfigureServiceCollection(services =>
        {
            services.AddSingleton<IExceptionHandler, ClientExceptionHandler>();
            services.AddSingleton<IExceptionHandler, SampleExceptionHandler>();
        });

        configurator.ConfigureAutoPersistenceModel(model =>
        {
            model.Override<Entity>(x => x.Map(e => e.String).Length(500));
            model.Override<Entity>(x => x.Map(e => e.Unique).Column("UniqueString").Unique());
        });

        configurator.ConfigureSwaggerGenOptions(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc("samples", new() { Title = "Samples", Version = "v1" });
            swaggerGenOptions.SwaggerDoc("external", new() { Title = "External", Version = "v1" });

            swaggerGenOptions.DocInclusionPredicate((documentName, api) =>
                documentName == "samples" ||
                documentName == "external" && api.GroupName == "ExternalSamples"
            );

            swaggerGenOptions.AddSecurityDefinition("Custom",
                new()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "X-Custom-API-Key",
                    Description = "Demonstration of additional security definitions",
                },
                documentName: "external"
            );
            swaggerGenOptions.AddSecurityDefinition("Custom.Secret",
                new()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "X-Custom-API-Secret",
                    Description = "Demonstration of adding two requirements",
                },
                documentName: "external"
            );

            swaggerGenOptions.AddSecurityRequirementToOperationsThatUse<AuthorizeAttribute>(["Custom", "Custom.Secret"], documentName: "external");
            swaggerGenOptions.AddParameterToOperationsThatUse<AuthorizeAttribute>(new() { Name = "X-Security-2", In = ParameterLocation.Header }, documentName: "external");
            swaggerGenOptions.AddParameterToOperationsThatUse<AuthorizeAttribute>(new() { Name = "X-Security-1", In = ParameterLocation.Header }, position: 0, documentName: "external");
        });

        configurator.ConfigureSwaggerUIOptions(swaggerUIOptions =>
        {
            swaggerUIOptions.SwaggerEndpoint($"samples/swagger.json", "Samples");
            swaggerUIOptions.SwaggerEndpoint($"external/swagger.json", "External");
        });

        configurator.ConfigureLayoutDescriptors(layouts =>
        {
            configurator.UsingDomainModel(domain =>
            {
                var rtwd = domain.Types[typeof(RichTransientWithData)];
                var rtwdRoute = rtwd.GetInitializerActionModel().GetRoute();
                var rtwdDetail = rtwd.Get<DetailPage>();
                var rtwdPageDetail = (PageTitle)(rtwdDetail.Header?.Schema ?? throw new("`RichTransientWithData` is expected to have `PageTitle` in `Header`"));

                layouts.Add(DefaultLayout("default",
                    sideMenu: SideMenu(
                        menu:
                        [
                            SideMenuItem("/", "pi pi-home"),
                            SideMenuItem("/specs", "pi pi-list-check", title: "Specs")
                        ],
                        footer: String(Inline("FT"))
                    ),
                    header: Header(
                        siteMap:
                        [
                            HeaderItem("/", icon: "pi pi-home"),
                            HeaderItem($"/{rtwdRoute}", title: rtwdPageDetail.Title),
                            HeaderItem("/specs", icon: "pi pi-list-check", title: "Specs"),
                            HeaderItem("/specs/card-link", title: "Card Link", parentRoute: "/specs"),
                            HeaderItem("/specs/custom-css", title: "Custom CSS", parentRoute: "/specs"),
                            HeaderItem("/specs/data-panel", title: "Data Panel", parentRoute: "/specs"),
                            HeaderItem("/specs/detail-page", title: "Detail Page", parentRoute: "/specs"),
                            HeaderItem("/specs/header", title: "Header", parentRoute: "/specs"),
                            HeaderItem("/specs/locale", title: "Locale", parentRoute: "/specs"),
                            HeaderItem("/specs/menu-page", title: "Menu Page", parentRoute: "/specs"),
                            HeaderItem("/specs/page-title", title: "Page Title", parentRoute: "/specs"),
                            HeaderItem("/specs/side-menu", title: "Side Menu", parentRoute: "/specs"),
                            HeaderItem("/specs/toast", title: "Toast", parentRoute: "/specs")
                        ]
                    )
                ));
            });
        });

        configurator.ConfigurePageDescriptors(pages =>
        {
            var headers = Inline(new { Authorization = "token-jane" });
            foreach (var page in pages.OfType<ComponentDescriptorAttribute<DetailPage>>())
            {
                if (page.Data is not RemoteData remote) { continue; }

                remote.Headers = headers;
            }

            configurator.UsingDomainModel(domain =>
            {
                var rtwdRoute = domain.Types[typeof(RichTransientWithData)].GetInitializerActionModel().GetRoute();
                var timeRoute = domain.Types[typeof(TimeProviderSamples)].GetMembers().Methods[nameof(TimeProviderSamples.GetNow)].GetSingle<ActionModelAttribute>().GetRoute();

                pages.Add(MenuPage("index",
                    header: DataPanel("Expand to see server time", String(Remote($"/{timeRoute}", headers: headers)),
                        collapsed: true
                    ),
                    links:
                    [
                        CardLink($"/{rtwdRoute.Replace("{id}", "test1")}", "Rich Transient w/ Data 1"),
                        CardLink($"/{rtwdRoute.Replace("{id}", "test2")}", "Rich Transient w/ Data 2"),
                        CardLink($"/{rtwdRoute.Replace("{id}", "test3")}", "Rich Transient w/ Data 3")
                    ]
                ));
            });

            pages.Add(MenuPage("specs",
                header: PageTitle(
                  title: "Specs",
                  description: "All UI Specs are listed here"
                ),
                links:
                [
                    CardLink("/specs/card-link", "Card Link",
                        icon: "pi pi-microchip",
                        description: "A big card link component to render links in menu-like pages"
                    ),
                    CardLink("/specs/custom-css", "Custom CSS",
                        icon: "pi pi-microchip",
                        description: "Allow custom configuration to define custom css and more"
                    ),
                    CardLink("/specs/data-panel", "Data Panel",
                        icon: "pi pi-microchip",
                        description: "A page component to lazy load and view a data within a panel"
                    ),
                    CardLink("/specs/detail-page", "Detail Page",
                        icon: "pi pi-microchip",
                        description: "A page component suitable for rendering entities and rich transients"
                    ),
                    CardLink("/specs/header", "Header",
                        icon: "pi pi-microchip",
                        description: "A layout component that renders a breadcrumb"
                    ),
                    CardLink("/specs/locale", "Locale",
                        icon: "pi pi-microchip",
                        description: "Allow locale customization and language support"
                    ),
                    CardLink("/specs/menu-page", "Menu Page",
                        icon: "pi pi-microchip",
                        description: "A page component suitable for rendering navigation pages"
                    ),
                    CardLink("/specs/page-title", "Page Title",
                        icon: "pi pi-microchip",
                        description: "A page component to render page title, desc and actions"
                    ),
                    CardLink("/specs/side-menu", "Side Menu",
                        icon: "pi pi-microchip",
                        description: "A layout component to render application menu"
                    ),
                    CardLink("/specs/toast", "Toast",
                        icon: "pi pi-microchip",
                        description: "A behavioral component to render alert messages"
                    )
                ]
            ));
        });
    }
}