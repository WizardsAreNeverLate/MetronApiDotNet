using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metron.Api.Client;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Metron.Api.Tests;

[Category("Live")]
[RequiresToken]
[NotInParallel("metron-live")]
public class LiveTests
{
    private static MetronClient NewClient() => MetronClientFactory.Create(LiveToken.Find()!);

    [Test]
    public async Task Publisher_list_returns_a_non_empty_page()
    {
        var page = await NewClient().Api.Publisher.EmptyPathSegment.GetAsync(c => c.QueryParameters.Page = 1);

        await Assert.That(page!.Count!.Value).IsGreaterThan(0);
        await Assert.That(page.Results!).IsNotEmpty();
        await Assert.That(page.Results![0].Id).IsNotNull();
        await Assert.That(page.Results[0].Name).IsNotNull();
    }

    [Test]
    public async Task Series_search_by_name_returns_matching_series()
    {
        var page = await NewClient().Api.Series.EmptyPathSegment.GetAsync(c => c.QueryParameters.Name = "Batman");

        await Assert.That(page!.Results!).IsNotEmpty();
        await Assert.That(page.Results!.All(s => s.Series!.Contains("Batman", StringComparison.OrdinalIgnoreCase)))
            .IsTrue();
    }

    [Test]
    public async Task Series_issue_list_and_issue_detail_round_trip()
    {
        var client = NewClient();
        var series = await client.Api.Series.EmptyPathSegment.GetAsync(c => c.QueryParameters.Name = "Batman");
        var seriesId = series!.Results!.First().Id!.Value;

        var issues = await client.Api.Series[seriesId].Issue_list.EmptyPathSegment.GetAsync();
        await Assert.That(issues!.Results!).IsNotEmpty();

        var issueId = issues.Results!.First().Id!.Value;
        var issue = await client.Api.Issue[issueId].EmptyPathSegment.GetAsync();

        await Assert.That(issue!.Id).IsEqualTo(issueId);
        await Assert.That(issue.Series).IsNotNull();
        await Assert.That(issue.Publisher).IsNotNull();
    }
}

[Category("Live")]
[RequiresToken]
[NotInParallel("metron-live")]
public class LiveConvenienceTests
{
    private static MetronApi NewApi() => MetronApi.Create(LiveToken.Find()!);

    [Test]
    public async Task Stream_yields_publishers_across_the_first_page()
    {
        var names = new List<string>();
        await foreach (var publisher in NewApi().Publishers.StreamAsync())
        {
            names.Add(publisher.Name);
            if (names.Count == 3) break;
        }

        await Assert.That(names.Count).IsEqualTo(3);
        await Assert.That(names.All(n => n.Length > 0)).IsTrue();
    }

    [Test]
    public async Task Series_search_issue_list_and_detail_round_trip()
    {
        var api = NewApi();
        var series = (await api.Series.ListAsync(new SeriesFilter { Name = "Batman" })).Items.First();

        var issues = await api.Series.ListIssuesAsync(series.Id);
        var issue = await api.Issues.GetAsync(issues.Items.First().Id);

        await Assert.That(issue.Id).IsEqualTo(issues.Items.First().Id);
        await Assert.That(issue.Series).IsNotNull();
        await Assert.That(issue.Publisher).IsNotNull();
    }
}
