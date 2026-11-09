using System;
using System.Net.Http;
using Metron.Api.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Metron.Api
{
    /// <summary>Creates a ready-to-use <see cref="MetronClient"/> without a DI container.</summary>
    public static class MetronClientFactory
    {
        public static MetronClient Create(string token) =>
            Create(new MetronClientOptions { Token = token });

        public static MetronClient Create(MetronClientOptions options, HttpClient? httpClient = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Validate();

            var client = httpClient ?? KiotaClientFactory.Create();
            if (!client.DefaultRequestHeaders.UserAgent.TryParseAdd(options.UserAgent))
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Metron.Api.NET");

            var host = new Uri(options.BaseUrl).Host;
            var auth = new BaseBearerTokenAuthenticationProvider(
                new StaticAccessTokenProvider(options.Token!, host));
            var adapter = new HttpClientRequestAdapter(auth, httpClient: client)
            {
                BaseUrl = options.BaseUrl.TrimEnd('/')
            };
            return new MetronClient(adapter);
        }

        private sealed class StaticAccessTokenProvider : IAccessTokenProvider
        {
            private readonly string _token;

            public StaticAccessTokenProvider(string token, string host)
            {
                _token = token;
                AllowedHostsValidator = new AllowedHostsValidator(new[] { host });
            }

            public AllowedHostsValidator AllowedHostsValidator { get; }

            public System.Threading.Tasks.Task<string> GetAuthorizationTokenAsync(
                Uri uri, System.Collections.Generic.Dictionary<string, object>? additionalAuthenticationContext = null,
                System.Threading.CancellationToken cancellationToken = default) =>
                System.Threading.Tasks.Task.FromResult(_token);
        }
    }
}
