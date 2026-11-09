# Metron.Api

Typed .NET client for the [Metron](https://metron.cloud) comic book database. The client code is generated from `docs/Metron Comicbook Database.yaml` with [Kiota](https://learn.microsoft.com/openapi/kiota/). The library targets `netstandard2.0` and `net10.0`.

## Use

`IMetronApi` is a small wrapper over the generated client. It returns plain DTOs with non-nullable ids and names, takes filter objects, and pages for you.

```csharp
IMetronApi metron = MetronApi.Create(token);

var publisher = await metron.Publishers.GetAsync(1);
var firstPage = await metron.Series.ListAsync(new SeriesFilter { Name = "Batman", PublisherId = 1 });
Console.WriteLine($"{firstPage.TotalCount} matches, more pages: {firstPage.HasNext}");

await foreach (var series in metron.Series.StreamAsync(new SeriesFilter { Name = "Batman" }))
    Console.WriteLine($"{series.Id} {series.Name}");

var issues = await metron.Series.ListIssuesAsync(seriesId);
var issue = await metron.Issues.GetAsync(issues.Items[0].Id);
```

Every resource (`Publishers`, `Series`, `Issues`, `Characters`, `Creators`, `Teams`, `Arcs`, `Universes`, `Imprints`) offers `GetAsync`, `ListAsync` and `StreamAsync`. `Roles` and `SeriesTypes` offer list and stream only. Series, characters, teams and arcs add `ListIssuesAsync` and `StreamIssues`, and `Publishers` adds `ListSeriesAsync` and `StreamSeries`. `StreamAsync` requests the next page only when you enumerate that far, and every method takes a `CancellationToken`.

With dependency injection (registers `MetronClient` and `IMetronApi`, and adds retry on HTTP 429 through Kiota handlers):

```csharp
services.AddMetronClient(o => o.Token = configuration["Metron:Token"]);
```

Failed calls, including a 404 from `GetAsync`, throw `Microsoft.Kiota.Abstractions.ApiException` with `ResponseStatusCode`. Metron rate-limits requests, so prefer the DI path for built-in retry. The wrapper covers the read-only catalog. Collection, pull list, reading list and wish list endpoints are reachable through `metron.Raw`.

## Use the Kiota client directly

`IMetronApi.Raw` returns the generated `MetronClient`. You can also create it without the wrapper:

```csharp
var client = MetronClientFactory.Create(token);

var page = await client.Api.Publisher.EmptyPathSegment.GetAsync();
var batman = await client.Api.Series.EmptyPathSegment.GetAsync(r => r.QueryParameters.Name = "Batman");
var issue = await client.Api.Issue[127884].EmptyPathSegment.GetAsync();
```

Kiota names collection endpoints `.EmptyPathSegment`, and its model properties are nullable (`Id` is `int?`).

## Regenerate

```
cd src/Metron.Api
kiota generate -l CSharp -d ../../docs/"Metron Comicbook Database.yaml" -o Generated -n Metron.Api.Client -c MetronClient --exclude-backward-compatible --clean-output
```

## Sample and tests

```
export METRON_TOKEN=...   # or place it in secrets/tokens.txt
dotnet run --project samples/Metron.Api.Sample
dotnet test --solution MetronApi.slnx   # TUnit; live tests skip without a token
```

`global.json` enables the Microsoft.Testing.Platform runner that TUnit needs on the .NET 10+ SDK.
