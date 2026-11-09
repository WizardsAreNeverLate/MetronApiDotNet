using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Metron.Api;
using Metron.Api.Client;
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
var client = provider.GetRequiredService<MetronClient>();

try
{
    Console.WriteLine("== Publishers (page 1) ==");
    var publishers = await client.Api.Publisher.EmptyPathSegment.GetAsync(c => c.QueryParameters.Page = 1);
    Console.WriteLine($"{publishers?.Count} publishers in total");
    foreach (var p in publishers?.Results?.Take(5) ?? [])
        Console.WriteLine($"  [{p.Id}] {p.Name}");

    Console.WriteLine();
    Console.WriteLine("== Series named \"Batman\" ==");
    var seriesList = new System.Collections.Generic.List<Metron.Api.Client.Models.SeriesList>();
    for (var page = 1; page <= 2; page++)
    {
        var result = await client.Api.Series.EmptyPathSegment.GetAsync(c =>
        {
            c.QueryParameters.Name = "Batman";
            c.QueryParameters.Page = page;
        });
        Console.WriteLine($"page {page}: {result?.Results?.Count ?? 0} of {result?.Count} matches");
        seriesList.AddRange(result?.Results ?? []);
        if (result?.Next is null) break;
    }
    foreach (var s in seriesList.Take(8))
        Console.WriteLine($"  [{s.Id}] {s.Series}, {s.Publisher?.Name}, issues: {s.IssueCount}");

    var first = seriesList.FirstOrDefault(s => s.Id is not null);
    if (first is null)
    {
        Console.WriteLine("No series found, skipping issue calls.");
        return 0;
    }

    Console.WriteLine();
    Console.WriteLine($"== Issues of [{first.Id}] {first.Series} (page 1) ==");
    var issues = await client.Api.Series[first.Id!.Value].Issue_list.EmptyPathSegment
        .GetAsync(c => c.QueryParameters.Page = 1);
    foreach (var i in issues?.Results?.Take(5) ?? [])
        Console.WriteLine($"  [{i.Id}] {i.Issue} (cover {i.CoverDate})");

    var firstIssue = issues?.Results?.FirstOrDefault(i => i.Id is not null);
    if (firstIssue is not null)
    {
        Console.WriteLine();
        Console.WriteLine($"== Issue detail [{firstIssue.Id}] ==");
        var issue = await client.Api.Issue[firstIssue.Id!.Value].EmptyPathSegment.GetAsync();
        Console.WriteLine($"  Title:     {issue?.Series?.Name} #{issue?.Number}");
        Console.WriteLine($"  Publisher: {issue?.Publisher?.Name}");
        Console.WriteLine($"  Cover:     {issue?.CoverDate}");
        Console.WriteLine($"  Credits:   {issue?.Credits?.Count ?? 0}, characters: {issue?.Characters?.Count ?? 0}");
        var desc = issue?.Desc;
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
