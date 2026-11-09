using System;
using System.Net.Http;
using Metron.Api.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Metron.Api
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers <see cref="MetronClient"/> as a singleton backed by <c>IHttpClientFactory</c>.</summary>
        public static IServiceCollection AddMetronClient(
            this IServiceCollection services, Action<MetronClientOptions> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);

            var http = services.AddHttpClient(nameof(MetronClient));
            http.ConfigurePrimaryHttpMessageHandler(() => KiotaClientFactory.GetDefaultHttpMessageHandler());
            var handlerCount = KiotaClientFactory.CreateDefaultHandlers().Count;
            for (var i = 0; i < handlerCount; i++)
            {
                var index = i;
                http.AddHttpMessageHandler(() => KiotaClientFactory.CreateDefaultHandlers()[index]);
            }

            services.AddSingleton(sp => MetronClientFactory.Create(
                sp.GetRequiredService<IOptions<MetronClientOptions>>().Value,
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(MetronClient))));
            return services;
        }
    }
}
