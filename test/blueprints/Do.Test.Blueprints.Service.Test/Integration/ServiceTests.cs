﻿using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Do.Test.Integration;

public class ServiceTests : IntegrationSpec<ServiceTests>
{
    public override void Run()
    {
        Forge.New
            .Service(
                business: c => c.Default(),
                database: c => c.MySql().ForDevelopment(c.Sqlite()),
                exceptionHandling: ex => ex.Default(typeUrlFormat: "https://do.mouseless.codes/errors/{0}"),
                configure: app => app.Features.AddConfigurationOverrider()
            )
            .Run();
    }

    static (bool, int)[] _testExceptionSuccessCases = [(true, 400), (false, 500)];

    [Test]
    public async Task Singleton_test_exception([ValueSource(nameof(_testExceptionSuccessCases))] (bool handled, int code) successCase)
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsync($"singleton/test-exception?handled={successCase.handled}", null);

        var problemDetails = response.Content.ReadFromJsonAsync<ProblemDetails>().Result;

        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(successCase.code);
    }
}
