using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metron.Api.Client;

namespace Metron.Api
{
    /// <summary>
    /// Read-only access to the Metron catalog with plain DTOs and automatic paging.
    /// Every call honors its <see cref="CancellationToken"/>. A failed call throws
    /// <see cref="Microsoft.Kiota.Abstractions.ApiException"/> with the HTTP status in <c>ResponseStatusCode</c>,
    /// including 404 from <c>GetAsync</c>.
    /// </summary>
    public interface IMetronApi
    {
        IPublisherResource Publishers { get; }
        ISeriesResource Series { get; }
        IReadResource<IssueSummary, Issue, IssueFilter> Issues { get; }
        ICharacterResource Characters { get; }
        IReadResource<CreatorSummary, Creator, NamedEntityFilter> Creators { get; }
        ITeamResource Teams { get; }
        IArcResource Arcs { get; }
        IReadResource<UniverseSummary, Universe, UniverseFilter> Universes { get; }
        IReadResource<ImprintSummary, Imprint, NamedEntityFilter> Imprints { get; }
        IListResource<Role, LookupFilter> Roles { get; }
        IListResource<SeriesType, LookupFilter> SeriesTypes { get; }

        /// <summary>The generated Kiota client, for endpoints this wrapper does not cover.</summary>
        MetronClient Raw { get; }
    }

    /// <summary>A catalog resource that can be listed and streamed.</summary>
    public interface IListResource<TSummary, in TFilter> where TFilter : class
    {
        /// <summary>Fetches one page. <paramref name="page"/> is one-based.</summary>
        Task<Page<TSummary>> ListAsync(TFilter? filter = null, int page = 1, CancellationToken cancellationToken = default);

        /// <summary>Yields every match, requesting further pages as the caller enumerates.</summary>
        IAsyncEnumerable<TSummary> StreamAsync(TFilter? filter = null, CancellationToken cancellationToken = default);
    }

    /// <summary>A catalog resource that also has a detail endpoint.</summary>
    public interface IReadResource<TSummary, TDetail, in TFilter> : IListResource<TSummary, TFilter>
        where TFilter : class
    {
        Task<TDetail> GetAsync(int id, CancellationToken cancellationToken = default);
    }

    /// <summary>A resource that exposes the issues it appears in.</summary>
    public interface IHasIssues
    {
        Task<Page<IssueSummary>> ListIssuesAsync(int id, int page = 1, CancellationToken cancellationToken = default);

        IAsyncEnumerable<IssueSummary> StreamIssues(int id, CancellationToken cancellationToken = default);
    }

    public interface IPublisherResource : IReadResource<PublisherSummary, Publisher, PublisherFilter>
    {
        Task<Page<SeriesSummary>> ListSeriesAsync(int publisherId, int page = 1, CancellationToken cancellationToken = default);

        IAsyncEnumerable<SeriesSummary> StreamSeries(int publisherId, CancellationToken cancellationToken = default);
    }

    public interface ISeriesResource : IReadResource<SeriesSummary, Series, SeriesFilter>, IHasIssues
    {
    }

    public interface ICharacterResource : IReadResource<CharacterSummary, Character, NamedEntityFilter>, IHasIssues
    {
    }

    public interface ITeamResource : IReadResource<TeamSummary, Team, NamedEntityFilter>, IHasIssues
    {
    }

    public interface IArcResource : IReadResource<ArcSummary, Arc, NamedEntityFilter>, IHasIssues
    {
    }
}
