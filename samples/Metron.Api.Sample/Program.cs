using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Metron.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using Spectre.Console;

var token = Metron.Api.Sample.TokenSource.Find();
if (token is null)
{
    AnsiConsole.MarkupLine("[red]No token found.[/] Set METRON_TOKEN or put it in secrets/tokens.txt.");
    return 1;
}

var services = new ServiceCollection();
services.AddMetronClient(o =>
{
    o.Token = token;
    o.UserAgent = "Metron.Api.Sample";
});
using var provider = services.BuildServiceProvider();
var metron = provider.GetRequiredService<IMetronApi>();

static string E(string? text) => Markup.Escape(text ?? string.Empty);

try
{
    AnsiConsole.Write(new Rule("Publishers (page 1)").LeftJustified());
    var publishers = await AnsiConsole.Status().StartAsync("Loading publishers...", _ => metron.Publishers.ListAsync());
    var publisherTable = new Table().RoundedBorder().AddColumn("Id").AddColumn("Name");
    foreach (var p in publishers.Items.Take(5))
        publisherTable.AddRow(p.Id.ToString(), E(p.Name));
    publisherTable.Caption($"{publishers.TotalCount} publishers in total");
    AnsiConsole.Write(publisherTable);

    AnsiConsole.Write(new Rule("Series named \"Batman\" (streamed, first 8)").LeftJustified());
    var batman = await AnsiConsole.Status().StartAsync("Searching series...", async _ =>
    {
        var found = new List<SeriesSummary>();
        await foreach (var s in metron.Series.StreamAsync(new SeriesFilter { Name = "Batman" }))
        {
            found.Add(s);
            if (found.Count == 8) break;
        }
        return found;
    });
    var seriesTable = new Table().RoundedBorder()
        .AddColumn("Id").AddColumn("Series").AddColumn("Publisher").AddColumn(new TableColumn("Issues").RightAligned());
    foreach (var s in batman)
        seriesTable.AddRow(s.Id.ToString(), E(s.Name), E(s.Publisher?.Name), s.IssueCount.ToString());
    AnsiConsole.Write(seriesTable);

    var first = batman.FirstOrDefault();
    if (first is null)
    {
        AnsiConsole.MarkupLine("[yellow]No series found, skipping issue calls.[/]");
        return 0;
    }

    AnsiConsole.Write(new Rule($"Issues of [[{first.Id}]] {E(first.Name)} (page 1)").LeftJustified());
    var issues = await AnsiConsole.Status().StartAsync("Loading issues...", _ => metron.Series.ListIssuesAsync(first.Id));
    var issueTable = new Table().RoundedBorder().AddColumn("Id").AddColumn("Issue").AddColumn("Cover date");
    foreach (var i in issues.Items.Take(5))
        issueTable.AddRow(i.Id.ToString(), E(i.Name), $"{i.CoverDate:yyyy-MM-dd}");
    AnsiConsole.Write(issueTable);

    var firstIssue = issues.Items.FirstOrDefault();
    if (firstIssue is not null)
    {
        var issue = await AnsiConsole.Status().StartAsync("Loading issue detail...", _ => metron.Issues.GetAsync(firstIssue.Id));
        var grid = new Grid().AddColumn().AddColumn();
        grid.AddRow("[grey]Title[/]", E(issue.Name));
        grid.AddRow("[grey]Publisher[/]", E(issue.Publisher?.Name));
        grid.AddRow("[grey]Cover[/]", $"{issue.CoverDate:yyyy-MM-dd}");
        grid.AddRow("[grey]Credits[/]", $"{issue.Credits.Count}, characters: {issue.Characters.Count}");
        var desc = issue.Desc;
        if (!string.IsNullOrWhiteSpace(desc))
            grid.AddRow("[grey]Summary[/]", E(desc.Length > 160 ? desc[..160] + "..." : desc));
        AnsiConsole.Write(new Panel(grid).Header($"Issue detail [[{issue.Id}]]").RoundedBorder());
    }
    return 0;
}
catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.TooManyRequests)
{
    AnsiConsole.MarkupLine("[red]Rate limited by Metron (HTTP 429).[/] Wait a minute and retry.");
    return 2;
}
catch (ApiException ex) when (ex.ResponseStatusCode is 401 or 403)
{
    AnsiConsole.MarkupLine($"[red]Metron rejected the token (HTTP {ex.ResponseStatusCode}).[/]");
    return 3;
}
catch (ApiException ex)
{
    AnsiConsole.MarkupLine($"[red]Metron returned HTTP {ex.ResponseStatusCode}:[/] {E(ex.Message)}");
    return 4;
}
