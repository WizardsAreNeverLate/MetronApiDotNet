using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metron.Api.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Metron.Api.Tests;

public class ConvenienceTests
{
    private const string Token = "test-token-123";

    private static string PublisherPage(int count, string? next, params (int id, string name)[] items) =>
        $$"""
        {
          "count": {{count}},
          "next": {{(next is null ? "null" : $"\"{next}\"")}},
          "previous": null,
          "results": [{{string.Join(",", items.Select(i => $$"""{ "id": {{i.id}}, "name": "{{i.name}}", "modified": "2024-01-02T03:04:05Z" }"""))}}]
        }
        """;

    private static (MetronApi Api, RoutingHandler Handler) ApiFor(Func<HttpRequestMessage, string> respond)
    {
        var handler = new RoutingHandler(respond);
        var client = MetronClientFactory.Create(new MetronClientOptions { Token = Token }, new HttpClient(handler));
        return (new MetronApi(client), handler);
    }

    [Test]
    public async Task ListAsync_maps_a_page_to_plain_dtos()
    {
        var (api, _) = ApiFor(_ => PublisherPage(7, "https://metron.cloud/api/publisher/?page=2", (1, "DC Comics"), (2, "Marvel")));

        var page = await api.Publishers.ListAsync();

        await Assert.That(page.TotalCount).IsEqualTo(7);
        await Assert.That(page.PageNumber).IsEqualTo(1);
        await Assert.That(page.HasNext).IsTrue();
        await Assert.That(page.Items.Select(p => p.Name).ToArray()).IsEquivalentTo(new[] { "DC Comics", "Marvel" });
        await Assert.That(page.Items[0].Id).IsEqualTo(1);
        await Assert.That(page.Items[0].Modified).IsEqualTo(DateTimeOffset.Parse("2024-01-02T03:04:05Z"));
    }

    [Test]
    public async Task ListAsync_without_a_next_link_reports_no_further_page()
    {
        var (api, _) = ApiFor(_ => PublisherPage(1, null, (1, "DC Comics")));

        var page = await api.Publishers.ListAsync(page: 3);

        await Assert.That(page.HasNext).IsFalse();
        await Assert.That(page.PageNumber).IsEqualTo(3);
    }

    [Test]
    public async Task Series_summary_maps_nested_publisher_and_type()
    {
        const string body = """
            { "count": 1, "results": [ {
              "id": 10, "series": "Batman", "volume": 3, "year_began": 2016, "issue_count": 85,
              "publisher": { "id": 1, "name": "DC Comics" },
              "series_type": { "id": 5, "name": "Ongoing Series" } } ] }
            """;
        var (api, _) = ApiFor(_ => body);

        var series = (await api.Series.ListAsync()).Items.Single();

        await Assert.That(series.Id).IsEqualTo(10);
        await Assert.That(series.Name).IsEqualTo("Batman");
        await Assert.That(series.Volume).IsEqualTo(3);
        await Assert.That(series.YearEnd).IsNull();
        await Assert.That(series.IssueCount).IsEqualTo(85);
        await Assert.That(series.Publisher!.Name).IsEqualTo("DC Comics");
        await Assert.That(series.SeriesType!.Id).IsEqualTo(5);
    }

    [Test]
    public async Task Issue_detail_maps_credits_dates_and_relations()
    {
        const string body = """
            {
              "id": 99, "number": "7",
              "publisher": { "id": 1, "name": "DC Comics" },
              "series": { "id": 10, "name": "Batman", "volume": 3, "year_began": 2016 },
              "cover_date": "2020-05-01", "store_date": "2020-03-18",
              "desc": "A dark night.", "page": 32,
              "credits": [ { "id": 3, "creator": "Tom King", "role": [ { "id": 1, "name": "Writer" } ] } ],
              "characters": [ { "id": 8, "name": "Batman" } ],
              "teams": [ { "id": 9, "name": "Bat-Family" } ]
            }
            """;
        var (api, _) = ApiFor(_ => body);

        var issue = await api.Issues.GetAsync(99);

        await Assert.That(issue.Id).IsEqualTo(99);
        await Assert.That(issue.Name).IsEqualTo("Batman #7");
        await Assert.That(issue.CoverDate).IsEqualTo(new DateTime(2020, 5, 1));
        await Assert.That(issue.StoreDate).IsEqualTo(new DateTime(2020, 3, 18));
        await Assert.That(issue.FocDate).IsNull();
        await Assert.That(issue.PageCount).IsEqualTo(32);
        await Assert.That(issue.Credits.Single().Creator).IsEqualTo("Tom King");
        await Assert.That(issue.Credits.Single().Roles.Single().Name).IsEqualTo("Writer");
        await Assert.That(issue.Characters.Single().Name).IsEqualTo("Batman");
        await Assert.That(issue.Teams.Single().Id).IsEqualTo(9);
        await Assert.That(issue.Arcs).IsEmpty();
    }

    [Test]
    public async Task GetAsync_requests_the_detail_path_for_the_id()
    {
        var (api, handler) = ApiFor(_ => """{ "id": 42, "name": "Batman" }""");

        var series = await api.Series.GetAsync(42);

        await Assert.That(handler.Requests.Single().RequestUri!.AbsolutePath).IsEqualTo("/api/series/42/");
        await Assert.That(series.Id).IsEqualTo(42);
        await Assert.That(series.Name).IsEqualTo("Batman");
    }

    [Test]
    public async Task GetAsync_propagates_ApiException_on_404()
    {
        var handler = new FakeHandler("""{ "detail": "Not found." }""", HttpStatusCode.NotFound);
        var api = new MetronApi(MetronClientFactory.Create(new MetronClientOptions { Token = Token }, new HttpClient(handler)));

        var ex = await Assert.ThrowsAsync<ApiException>(async () => await api.Issues.GetAsync(1));

        await Assert.That(ex!.ResponseStatusCode).IsEqualTo(404);
    }

    [Test]
    public async Task Series_filter_becomes_query_parameters()
    {
        var (api, handler) = ApiFor(_ => """{ "count": 0, "results": [] }""");

        await api.Series.ListAsync(new SeriesFilter
        {
            Name = "Batman",
            PublisherId = 1,
            YearBegan = 2016,
            RoleIds = new[] { 1, 2 },
            ModifiedGt = DateTimeOffset.Parse("2024-01-01T00:00:00Z")
        }, page: 2);

        var uri = handler.Requests.Single().RequestUri!;
        await Assert.That(uri.AbsolutePath).IsEqualTo("/api/series/");
        await Assert.That(uri.Query).Contains("name=Batman");
        await Assert.That(uri.Query).Contains("publisher_id=1");
        await Assert.That(uri.Query).Contains("year_began=2016");
        await Assert.That(uri.Query).Contains("page=2");
        await Assert.That(uri.Query).Contains("role_id=1,2");
        await Assert.That(uri.Query).Contains("modified_gt=2024-01-01");
        await Assert.That(uri.Query).DoesNotContain("volume");
    }

    [Test]
    public async Task Issue_filter_sends_dates_as_calendar_days()
    {
        var (api, handler) = ApiFor(_ => """{ "count": 0, "results": [] }""");

        await api.Issues.ListAsync(new IssueFilter
        {
            SeriesId = 10,
            Number = "7",
            CoverDateAfter = new DateTime(2020, 1, 1),
            CoverDateBefore = new DateTime(2020, 12, 31),
            StoreDate = new DateTime(2020, 3, 18)
        });

        var query = handler.Requests.Single().RequestUri!.Query;
        await Assert.That(query).Contains("series_id=10");
        await Assert.That(query).Contains("number=7");
        await Assert.That(query).Contains("cover_date_range_after=2020-01-01");
        await Assert.That(query).Contains("cover_date_range_before=2020-12-31");
        await Assert.That(query).Contains("store_date=2020-03-18");
    }

    [Test]
    public async Task Null_filter_sends_only_the_page()
    {
        var (api, handler) = ApiFor(_ => """{ "count": 0, "results": [] }""");

        await api.Characters.ListAsync();

        await Assert.That(handler.Requests.Single().RequestUri!.Query).IsEqualTo("?page=1");
    }

    [Test]
    public async Task StreamAsync_follows_pages_until_there_is_no_next_link()
    {
        var (api, handler) = ApiFor(request =>
            request.RequestUri!.Query.Contains("page=1")
                ? PublisherPage(3, "https://metron.cloud/api/publisher/?page=2", (1, "A"), (2, "B"))
                : PublisherPage(3, null, (3, "C")));

        var names = new List<string>();
        await foreach (var publisher in api.Publishers.StreamAsync())
            names.Add(publisher.Name);

        await Assert.That(names).IsEquivalentTo(new[] { "A", "B", "C" });
        await Assert.That(handler.Requests.Count).IsEqualTo(2);
    }

    [Test]
    public async Task StreamAsync_is_lazy_and_stops_requesting_when_the_caller_stops()
    {
        var (api, handler) = ApiFor(_ =>
            PublisherPage(99, "https://metron.cloud/api/publisher/?page=next", (1, "A"), (2, "B")));

        var first = new List<PublisherSummary>();
        await foreach (var publisher in api.Publishers.StreamAsync())
        {
            first.Add(publisher);
            if (first.Count == 2) break;
        }

        await Assert.That(handler.Requests.Count).IsEqualTo(1);
    }

    [Test]
    public async Task StreamAsync_stops_on_an_empty_page_even_with_a_next_link()
    {
        var (api, handler) = ApiFor(_ => PublisherPage(0, "https://metron.cloud/api/publisher/?page=2"));

        var count = 0;
        await foreach (var _ in api.Publishers.StreamAsync())
            count++;

        await Assert.That(count).IsEqualTo(0);
        await Assert.That(handler.Requests.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Series_issue_helpers_use_the_issue_list_path_and_page()
    {
        const string body = """
            { "count": 1, "results": [ { "id": 5, "issue": "Batman (2016) #1", "number": "1", "cover_date": "2016-06-01",
              "series": { "id": 10, "name": "Batman", "volume": 3, "year_began": 2016 } } ] }
            """;
        var (api, handler) = ApiFor(_ => body);

        var page = await api.Series.ListIssuesAsync(10, page: 4);

        var uri = handler.Requests.Single().RequestUri!;
        await Assert.That(uri.AbsolutePath).IsEqualTo("/api/series/10/issue_list/");
        await Assert.That(uri.Query).Contains("page=4");
        await Assert.That(page.Items.Single().Name).IsEqualTo("Batman (2016) #1");
        await Assert.That(page.Items.Single().CoverDate).IsEqualTo(new DateTime(2016, 6, 1));
        await Assert.That(page.Items.Single().Series!.Id).IsEqualTo(10);
    }

    [Test]
    [Arguments("character", "Characters")]
    [Arguments("team", "Teams")]
    [Arguments("arc", "Arcs")]
    public async Task Other_issue_sources_use_their_own_issue_list_path(string segment, string resource)
    {
        var (api, handler) = ApiFor(_ => """{ "count": 0, "results": [] }""");
        IHasIssues source = resource switch
        {
            "Characters" => api.Characters,
            "Teams" => api.Teams,
            _ => api.Arcs
        };

        await source.ListIssuesAsync(7);

        await Assert.That(handler.Requests.Single().RequestUri!.AbsolutePath).IsEqualTo($"/api/{segment}/7/issue_list/");
    }

    [Test]
    public async Task Publisher_series_helper_uses_the_series_list_path()
    {
        var (api, handler) = ApiFor(_ => """{ "count": 0, "results": [] }""");

        await api.Publishers.ListSeriesAsync(1);

        await Assert.That(handler.Requests.Single().RequestUri!.AbsolutePath).IsEqualTo("/api/publisher/1/series_list/");
    }

    [Test]
    public async Task StreamIssues_follows_pages_of_a_relationship()
    {
        var (api, handler) = ApiFor(request =>
            request.RequestUri!.Query.Contains("page=1")
                ? """{ "count": 2, "next": "x", "results": [ { "id": 1, "issue": "A #1" } ] }"""
                : """{ "count": 2, "next": null, "results": [ { "id": 2, "issue": "A #2" } ] }""");

        var ids = new List<int>();
        await foreach (var issue in api.Series.StreamIssues(10))
            ids.Add(issue.Id);

        await Assert.That(ids).IsEquivalentTo(new[] { 1, 2 });
        await Assert.That(handler.Requests.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Roles_and_series_types_list_lookups()
    {
        var (api, handler) = ApiFor(request => request.RequestUri!.AbsolutePath.Contains("series_type")
            ? """{ "count": 1, "results": [ { "id": 5, "name": "Ongoing Series" } ] }"""
            : """{ "count": 1, "results": [ { "id": 1, "name": "Writer" } ] }""");

        var roles = await api.Roles.ListAsync(new LookupFilter { Name = "Writer" });
        var types = await api.SeriesTypes.ListAsync();

        await Assert.That(roles.Items.Single().Name).IsEqualTo("Writer");
        await Assert.That(types.Items.Single().Name).IsEqualTo("Ongoing Series");
        await Assert.That(handler.Requests[0].RequestUri!.Query).Contains("name=Writer");
    }

    [Test]
    public async Task A_cancelled_token_aborts_the_stream()
    {
        var (api, handler) = ApiFor(_ => PublisherPage(1, null, (1, "A")));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in api.Publishers.StreamAsync(cancellationToken: cts.Token))
            {
            }
        });
        await Assert.That(handler.Requests.Count).IsEqualTo(0);
    }

    [Test]
    public async Task AddMetronClient_also_registers_IMetronApi()
    {
        var handler = new RoutingHandler(_ => PublisherPage(1, null, (1, "DC Comics")));
        var services = new ServiceCollection();
        services.AddMetronClient(o => o.Token = Token);
        services.AddHttpClient(nameof(MetronClient)).ConfigurePrimaryHttpMessageHandler(() => handler);
        using var provider = services.BuildServiceProvider();

        var api = provider.GetRequiredService<IMetronApi>();
        var page = await api.Publishers.ListAsync();

        await Assert.That(page.Items.Single().Name).IsEqualTo("DC Comics");
        await Assert.That(handler.Requests.Single().Headers.Authorization?.Parameter).IsEqualTo(Token);
        await Assert.That(api.Raw).IsSameReferenceAs(provider.GetRequiredService<MetronClient>());
    }

    [Test]
    public async Task Create_builds_a_working_instance_from_a_token()
    {
        await Assert.That(MetronApi.Create(Token).Raw).IsNotNull();
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = new MetronApi(null!);
            await Task.CompletedTask;
        });
    }
}

internal sealed class RoutingHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, string> _respond;
    private readonly List<HttpRequestMessage> _requests = new();

    public RoutingHandler(Func<HttpRequestMessage, string> respond) => _respond = respond;

    public IReadOnlyList<HttpRequestMessage> Requests => _requests;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _requests.Add(request);
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_respond(request), Encoding.UTF8, "application/json"),
            RequestMessage = request
        });
    }
}
