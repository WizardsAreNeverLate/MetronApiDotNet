using System;

namespace Metron.Api
{
    /// <summary>Settings for connecting to the Metron API.</summary>
    public sealed class MetronClientOptions
    {
        public const string DefaultBaseUrl = "https://metron.cloud";

        /// <summary>Knox API token sent as a bearer token. Required.</summary>
        public string? Token { get; set; }

        /// <summary>Root URL of the Metron server, without the <c>/api</c> suffix.</summary>
        public string BaseUrl { get; set; } = DefaultBaseUrl;

        /// <summary>Value for the User-Agent header. Metron asks clients to identify themselves.</summary>
        public string UserAgent { get; set; } = "Metron.Api.NET";

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(Token))
                throw new InvalidOperationException("MetronClientOptions.Token is required.");
            if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
                throw new InvalidOperationException("MetronClientOptions.BaseUrl must be an absolute URL.");
        }
    }
}
