using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Metron.Api.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Metron.Api.Tests;

public class OfflineTests
{
    private const string Token = "test-token-123";

    private const string PublisherPage = """
        {
          "count": 2,
          "next": "https://metron.cloud/api/publisher/?page=2",
          "previous": null,
          "results": [
            { "id": 1, "name": "DC Comics", "modified": "2024-01-02T03:04:05Z" },
            { "id": 2, "name": "Marvel", "modified": "2024-01-02T03:04:05Z" }
          ]
        }
        """;

    private static MetronClient ClientFor(FakeHandler handler) =>
        MetronClientFactory.Create(new MetronClientOptions { Token = Token }, new HttpClient(handler));

    [Test]
    public async Task Request_carries_bearer_token_and_targets_metron_api()
    {
        var handler = new FakeHandler(PublisherPage);

        await ClientFor(handler).Api.Publisher.EmptyPathSegment.GetAsync();

        var request = handler.Request!;
        await Assert.That(request.Headers.Authorization?.Scheme).IsEqualTo("Bearer");
        await Assert.That(request.Headers.Authorization?.Parameter).IsEqualTo(Token);
        await Assert.That(request.Method).IsEqualTo(HttpMethod.Get);
        await Assert.That(request.RequestUri!.Host).IsEqualTo("metron.cloud");
        await Assert.That(request.RequestUri.Scheme).IsEqualTo("https");
        await Assert.That(request.RequestUri.AbsolutePath).IsEqualTo("/api/publisher/");
    }

    [Test]
    public async Task Request_identifies_the_client_with_a_user_agent()
    {
        var handler = new FakeHandler(PublisherPage);
        var options = new MetronClientOptions { Token = Token, UserAgent = "MyApp/1.0" };

        await MetronClientFactory.Create(options, new HttpClient(handler)).Api.Publisher.EmptyPathSegment.GetAsync();

        await Assert.That(handler.Request!.Headers.UserAgent.ToString()).Contains("MyApp/1.0");
    }

    [Test]
    public async Task Name_and_page_filters_become_query_parameters()
    {
        var handler = new FakeHandler("""{ "count": 0, "results": [] }""");

        await ClientFor(handler).Api.Series.EmptyPathSegment.GetAsync(c =>
        {
            c.QueryParameters.Name = "Batman";
            c.QueryParameters.Page = 3;
        });

        var uri = handler.Request!.RequestUri!;
        await Assert.That(uri.AbsolutePath).IsEqualTo("/api/series/");
        await Assert.That(uri.Query).Contains("name=Batman");
        await Assert.That(uri.Query).Contains("page=3");
    }

    [Test]
    public async Task Unset_filters_are_not_sent()
    {
        var handler = new FakeHandler(PublisherPage);

        await ClientFor(handler).Api.Publisher.EmptyPathSegment.GetAsync();

        await Assert.That(handler.Request!.RequestUri!.Query).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Series_issue_list_uses_the_series_id_in_the_path()
    {
        var handler = new FakeHandler("""{ "count": 0, "results": [] }""");

        await ClientFor(handler).Api.Series[42].Issue_list.EmptyPathSegment
            .GetAsync(c => c.QueryParameters.Page = 2);

        var uri = handler.Request!.RequestUri!;
        await Assert.That(uri.AbsolutePath).IsEqualTo("/api/series/42/issue_list/");
        await Assert.That(uri.Query).Contains("page=2");
    }

    [Test]
    public async Task Publisher_page_deserialises_into_typed_models()
    {
        var page = await ClientFor(new FakeHandler(PublisherPage)).Api.Publisher.EmptyPathSegment.GetAsync();

        await Assert.That(page!.Count).IsEqualTo(2);
        await Assert.That(page.Next).IsEqualTo("https://metron.cloud/api/publisher/?page=2");
        await Assert.That(page.Previous).IsNull();
        await Assert.That(page.Results!.Select(p => p.Name!).ToArray())
            .IsEquivalentTo(new[] { "DC Comics", "Marvel" });
        await Assert.That(page.Results![0].Id).IsEqualTo(1);
        await Assert.That(page.Results[0].Modified).IsEqualTo(DateTimeOffset.Parse("2024-01-02T03:04:05Z"));
    }

    [Test]
    public async Task Issue_detail_deserialises_nested_objects()
    {
        const string body = """
            {
              "id": 99,
              "number": "7",
              "publisher": { "id": 1, "name": "DC Comics" },
              "series": { "id": 10, "name": "Batman", "volume": 3, "year_began": 2016 },
              "cover_date": "2020-05-01",
              "desc": "A dark night."
            }
            """;

        var issue = await ClientFor(new FakeHandler(body)).Api.Issue[99].EmptyPathSegment.GetAsync();

        await Assert.That(issue!.Id).IsEqualTo(99);
        await Assert.That(issue.Number).IsEqualTo("7");
        await Assert.That(issue.Publisher!.Name).IsEqualTo("DC Comics");
        await Assert.That(issue.Series!.Name).IsEqualTo("Batman");
        await Assert.That(issue.Series.YearBegan).IsEqualTo(2016);
        await Assert.That(issue.Desc).IsEqualTo("A dark night.");
    }

    [Test]
    [Arguments(HttpStatusCode.Unauthorized)]
    [Arguments(HttpStatusCode.NotFound)]
    [Arguments(HttpStatusCode.TooManyRequests)]
    [Arguments(HttpStatusCode.InternalServerError)]
    public async Task Non_success_status_throws_ApiException_with_the_status_code(HttpStatusCode status)
    {
        var client = ClientFor(new FakeHandler("""{ "detail": "nope" }""", status));

        var ex = await Assert.ThrowsAsync<ApiException>(async () => await client.Api.Publisher.EmptyPathSegment.GetAsync());

        await Assert.That(ex!.ResponseStatusCode).IsEqualTo((int)status);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task Missing_token_throws(string? token)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            MetronClientFactory.Create(new MetronClientOptions { Token = token }, new HttpClient(new FakeHandler()));
            await Task.CompletedTask;
        });
    }

    [Test]
    public async Task Custom_base_url_is_honoured()
    {
        var handler = new FakeHandler(PublisherPage);
        var options = new MetronClientOptions { Token = Token, BaseUrl = "https://example.test" };

        await MetronClientFactory.Create(options, new HttpClient(handler)).Api.Publisher.EmptyPathSegment.GetAsync();

        await Assert.That(handler.Request!.RequestUri!.Host).IsEqualTo("example.test");
    }

    [Test]
    public async Task AddMetronClient_resolves_a_working_client()
    {
        var handler = new FakeHandler(PublisherPage);
        var services = new ServiceCollection();
        services.AddMetronClient(o => o.Token = Token);
        services.AddHttpClient(nameof(MetronClient)).ConfigurePrimaryHttpMessageHandler(() => handler);
        using var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredService<MetronClient>();
        var page = await client.Api.Publisher.EmptyPathSegment.GetAsync();

        await Assert.That(page!.Results!.Count).IsEqualTo(2);
        await Assert.That(handler.Request!.Headers.Authorization?.Parameter).IsEqualTo(Token);
        await Assert.That(handler.Request.RequestUri!.Host).IsEqualTo("metron.cloud");
        await Assert.That(provider.GetRequiredService<MetronClient>()).IsSameReferenceAs(client);
    }
}
