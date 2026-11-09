# Metron.Api

Typed .NET client for the [Metron](https://metron.cloud) comic book database. The client code is generated from `docs/Metron Comicbook Database.yaml` with [Kiota](https://learn.microsoft.com/openapi/kiota/). The library targets `netstandard2.0` and `net10.0`.

## Use

```csharp
var client = MetronClientFactory.Create(token);

var page = await client.Api.Publisher.EmptyPathSegment.GetAsync();
var batman = await client.Api.Series.EmptyPathSegment.GetAsync(r => r.QueryParameters.Name = "Batman");
var issue = await client.Api.Issue[127884].EmptyPathSegment.GetAsync();
```

With dependency injection (adds retry on HTTP 429 through Kiota handlers):

```csharp
services.AddMetronClient(o => o.Token = configuration["Metron:Token"]);
```

Notes:
- Kiota names collection endpoints `.EmptyPathSegment`, and model properties are nullable (`Id` is `int?`).
- Metron rate-limits requests. Prefer the DI path for built-in retry.
- Failed calls throw `Microsoft.Kiota.Abstractions.ApiException` with `ResponseStatusCode`.

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
