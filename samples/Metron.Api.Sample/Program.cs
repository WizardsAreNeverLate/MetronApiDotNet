using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Metron.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;

var token = Metron.Api.Sample.TokenSource.Find();
if (token is null)
{
    Console.Error.WriteLine("No token found. Set METRON_TOKEN or put it in secrets/tokens.txt.");
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

try
{
    Console.WriteLine("== Publishers (page 1) ==");
    var publishers = await metron.Publishers.ListAsync();
    Console.WriteLine($"{publishers.TotalCount} publishers in total");
    foreach (var p in publishers.Items.Take(5))
        Console.WriteLine($"  [{p.Id}] {p.Name}");

    Console.WriteLine();
    Console.WriteLine("== Series named \"Batman\" (streamed, first 8) ==");
    var batman = new System.Collections.Generic.List<SeriesSummary>();
    await foreach (var s in metron.Series.StreamAsync(new SeriesFilter { Name = "Batman" }))
    {
        batman.Add(s);
        if (batman.Count == 8) break;
    }
    foreach (var s in batman)
        Console.WriteLine($"  [{s.Id}] {s.Name}, {s.Publisher?.Name}, issues: {s.IssueCount}");

    var first = batman.FirstOrDefault();
    if (first is null)
    {
        Console.WriteLine("No series found, skipping issue calls.");
        return 0;
    }

    Console.WriteLine();
    Console.WriteLine($"== Issues of [{first.Id}] {first.Name} (page 1) ==");
    var issues = await metron.Series.ListIssuesAsync(first.Id);
    foreach (var i in issues.Items.Take(5))
        Console.WriteLine($"  [{i.Id}] {i.Name} (cover {i.CoverDate:yyyy-MM-dd})");

    var firstIssue = issues.Items.FirstOrDefault();
    if (firstIssue is not null)
    {
        Console.WriteLine();
        Console.WriteLine($"== Issue detail [{firstIssue.Id}] ==");
        var issue = await metron.Issues.GetAsync(firstIssue.Id);
        Console.WriteLine($"  Title:     {issue.Name}");
        Console.WriteLine($"  Publisher: {issue.Publisher?.Name}");
        Console.WriteLine($"  Cover:     {issue.CoverDate:yyyy-MM-dd}");
        Console.WriteLine($"  Credits:   {issue.Credits.Count}, characters: {issue.Characters.Count}");
        var desc = issue.Desc;
        if (!string.IsNullOrWhiteSpace(desc))
            Console.WriteLine($"  Summary:   {(desc.Length > 160 ? desc[..160] + "..." : desc)}");
    }
    return 0;
}
catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.TooManyRequests)
{
    Console.Error.WriteLine("Rate limited by Metron (HTTP 429). Wait a minute and retry.");
    return 2;
}
catch (ApiException ex) when (ex.ResponseStatusCode is 401 or 403)
{
    Console.Error.WriteLine($"Metron rejected the token (HTTP {ex.ResponseStatusCode}).");
    return 3;
}
catch (ApiException ex)
{
    Console.Error.WriteLine($"Metron returned HTTP {ex.ResponseStatusCode}: {ex.Message}");
    return 4;
}
