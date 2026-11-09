using System;
using Metron.Api.Client;

namespace Metron.Api
{
    /// <summary>Default <see cref="IMetronApi"/> implementation over a <see cref="MetronClient"/>.</summary>
    public sealed class MetronApi : IMetronApi
    {
        public MetronApi(MetronClient client)
        {
            Raw = client ?? throw new ArgumentNullException(nameof(client));
            var api = client.Api;

            Publishers = new PublisherResource(
                async (f, p, ct) =>
                {
                    var r = await api.Publisher.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Publisher[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))),
                new Related<SeriesSummary>(async (id, p, ct) =>
                {
                    var r = await api.Publisher[id].Series_list.EmptyPathSegment
                        .GetAsync(x => x.QueryParameters.Page = p, ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                }));

            Series = new SeriesResource(
                async (f, p, ct) =>
                {
                    var r = await api.Series.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Series[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))),
                new Related<IssueSummary>(async (id, p, ct) =>
                {
                    var r = await api.Series[id].Issue_list.EmptyPathSegment
                        .GetAsync(x => x.QueryParameters.Page = p, ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                }));

            Issues = new ReadResource<IssueSummary, Issue, IssueFilter>(
                async (f, p, ct) =>
                {
                    var r = await api.Issue.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Issue[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))));

            Characters = new CharacterResource(
                async (f, p, ct) =>
                {
                    var r = await api.Character.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Character[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))),
                new Related<IssueSummary>(async (id, p, ct) =>
                {
                    var r = await api.Character[id].Issue_list.EmptyPathSegment
                        .GetAsync(x => x.QueryParameters.Page = p, ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                }));

            Creators = new ReadResource<CreatorSummary, Creator, NamedEntityFilter>(
                async (f, p, ct) =>
                {
                    var r = await api.Creator.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Creator[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))));

            Teams = new TeamResource(
                async (f, p, ct) =>
                {
                    var r = await api.Team.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Team[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))),
                new Related<IssueSummary>(async (id, p, ct) =>
                {
                    var r = await api.Team[id].Issue_list.EmptyPathSegment
                        .GetAsync(x => x.QueryParameters.Page = p, ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                }));

            Arcs = new ArcResource(
                async (f, p, ct) =>
                {
                    var r = await api.Arc.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Arc[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))),
                new Related<IssueSummary>(async (id, p, ct) =>
                {
                    var r = await api.Arc[id].Issue_list.EmptyPathSegment
                        .GetAsync(x => x.QueryParameters.Page = p, ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                }));

            Universes = new ReadResource<UniverseSummary, Universe, UniverseFilter>(
                async (f, p, ct) =>
                {
                    var r = await api.Universe.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Universe[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))));

            Imprints = new ReadResource<ImprintSummary, Imprint, NamedEntityFilter>(
                async (f, p, ct) =>
                {
                    var r = await api.Imprint.EmptyPathSegment
                        .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                    return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
                },
                async (id, ct) => Mapping.Map(Present(await api.Imprint[id].EmptyPathSegment
                    .GetAsync(cancellationToken: ct).ConfigureAwait(false))));

            Roles = new ListResource<Role, LookupFilter>(async (f, p, ct) =>
            {
                var r = await api.Role.EmptyPathSegment
                    .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
            });

            SeriesTypes = new ListResource<SeriesType, LookupFilter>(async (f, p, ct) =>
            {
                var r = await api.Series_type.EmptyPathSegment
                    .GetAsync(x => QueryMapping.Apply(x.QueryParameters, f, p), ct).ConfigureAwait(false);
                return Paging.Build(r?.Count, r?.Next, r?.Results, p, Mapping.Map);
            });
        }

        /// <summary>Creates an instance with default settings for the given Knox API token.</summary>
        public static MetronApi Create(string token) => new MetronApi(MetronClientFactory.Create(token));

        public MetronClient Raw { get; }
        public IPublisherResource Publishers { get; }
        public ISeriesResource Series { get; }
        public IReadResource<IssueSummary, Issue, IssueFilter> Issues { get; }
        public ICharacterResource Characters { get; }
        public IReadResource<CreatorSummary, Creator, NamedEntityFilter> Creators { get; }
        public ITeamResource Teams { get; }
        public IArcResource Arcs { get; }
        public IReadResource<UniverseSummary, Universe, UniverseFilter> Universes { get; }
        public IReadResource<ImprintSummary, Imprint, NamedEntityFilter> Imprints { get; }
        public IListResource<Role, LookupFilter> Roles { get; }
        public IListResource<SeriesType, LookupFilter> SeriesTypes { get; }

        private static T Present<T>(T? response) where T : class =>
            response ?? throw new InvalidOperationException("Metron returned an empty response body.");
    }
}
