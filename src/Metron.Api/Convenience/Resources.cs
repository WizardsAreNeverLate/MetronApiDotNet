using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Metron.Api
{
    internal delegate Task<Page<T>> PageFetcher<T>(int page, CancellationToken cancellationToken);

    internal static class Paging
    {
        public static Page<T> Build<TRaw, T>(
            int? count, string? next, List<TRaw>? results, int page, Func<TRaw, T> map)
        {
            var items = results == null ? new List<T>() : results.Select(map).ToList();
            return new Page<T>(items, count ?? items.Count, page, !string.IsNullOrEmpty(next));
        }

        public static async IAsyncEnumerable<T> Stream<T>(
            PageFetcher<T> fetch, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            for (var page = 1; ; page++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var current = await fetch(page, cancellationToken).ConfigureAwait(false);
                foreach (var item in current.Items)
                    yield return item;
                if (!current.HasNext || current.Items.Count == 0)
                    yield break;
            }
        }
    }

    internal sealed class Related<T>
    {
        private readonly Func<int, int, CancellationToken, Task<Page<T>>> _list;

        public Related(Func<int, int, CancellationToken, Task<Page<T>>> list) => _list = list;

        public Task<Page<T>> ListAsync(int id, int page, CancellationToken cancellationToken) =>
            _list(id, page, cancellationToken);

        public IAsyncEnumerable<T> Stream(int id, CancellationToken cancellationToken) =>
            Paging.Stream<T>((page, ct) => _list(id, page, ct), cancellationToken);
    }

    internal class ListResource<TSummary, TFilter> : IListResource<TSummary, TFilter>
        where TFilter : class
    {
        private readonly Func<TFilter?, int, CancellationToken, Task<Page<TSummary>>> _list;

        public ListResource(Func<TFilter?, int, CancellationToken, Task<Page<TSummary>>> list) => _list = list;

        public Task<Page<TSummary>> ListAsync(
            TFilter? filter = null, int page = 1, CancellationToken cancellationToken = default) =>
            _list(filter, page, cancellationToken);

        public IAsyncEnumerable<TSummary> StreamAsync(
            TFilter? filter = null, CancellationToken cancellationToken = default) =>
            Paging.Stream<TSummary>((page, ct) => _list(filter, page, ct), cancellationToken);
    }

    internal class ReadResource<TSummary, TDetail, TFilter>
        : ListResource<TSummary, TFilter>, IReadResource<TSummary, TDetail, TFilter>
        where TFilter : class
    {
        private readonly Func<int, CancellationToken, Task<TDetail>> _get;

        public ReadResource(
            Func<TFilter?, int, CancellationToken, Task<Page<TSummary>>> list,
            Func<int, CancellationToken, Task<TDetail>> get) : base(list) => _get = get;

        public Task<TDetail> GetAsync(int id, CancellationToken cancellationToken = default) =>
            _get(id, cancellationToken);
    }

    internal class IssueSourceResource<TSummary, TDetail, TFilter>
        : ReadResource<TSummary, TDetail, TFilter>, IHasIssues
        where TFilter : class
    {
        private readonly Related<IssueSummary> _issues;

        public IssueSourceResource(
            Func<TFilter?, int, CancellationToken, Task<Page<TSummary>>> list,
            Func<int, CancellationToken, Task<TDetail>> get,
            Related<IssueSummary> issues) : base(list, get) => _issues = issues;

        public Task<Page<IssueSummary>> ListIssuesAsync(
            int id, int page = 1, CancellationToken cancellationToken = default) =>
            _issues.ListAsync(id, page, cancellationToken);

        public IAsyncEnumerable<IssueSummary> StreamIssues(int id, CancellationToken cancellationToken = default) =>
            _issues.Stream(id, cancellationToken);
    }

    internal sealed class SeriesResource
        : IssueSourceResource<SeriesSummary, Series, SeriesFilter>, ISeriesResource
    {
        public SeriesResource(
            Func<SeriesFilter?, int, CancellationToken, Task<Page<SeriesSummary>>> list,
            Func<int, CancellationToken, Task<Series>> get,
            Related<IssueSummary> issues) : base(list, get, issues) { }
    }

    internal sealed class CharacterResource
        : IssueSourceResource<CharacterSummary, Character, NamedEntityFilter>, ICharacterResource
    {
        public CharacterResource(
            Func<NamedEntityFilter?, int, CancellationToken, Task<Page<CharacterSummary>>> list,
            Func<int, CancellationToken, Task<Character>> get,
            Related<IssueSummary> issues) : base(list, get, issues) { }
    }

    internal sealed class TeamResource
        : IssueSourceResource<TeamSummary, Team, NamedEntityFilter>, ITeamResource
    {
        public TeamResource(
            Func<NamedEntityFilter?, int, CancellationToken, Task<Page<TeamSummary>>> list,
            Func<int, CancellationToken, Task<Team>> get,
            Related<IssueSummary> issues) : base(list, get, issues) { }
    }

    internal sealed class ArcResource
        : IssueSourceResource<ArcSummary, Arc, NamedEntityFilter>, IArcResource
    {
        public ArcResource(
            Func<NamedEntityFilter?, int, CancellationToken, Task<Page<ArcSummary>>> list,
            Func<int, CancellationToken, Task<Arc>> get,
            Related<IssueSummary> issues) : base(list, get, issues) { }
    }

    internal sealed class PublisherResource
        : ReadResource<PublisherSummary, Publisher, PublisherFilter>, IPublisherResource
    {
        private readonly Related<SeriesSummary> _series;

        public PublisherResource(
            Func<PublisherFilter?, int, CancellationToken, Task<Page<PublisherSummary>>> list,
            Func<int, CancellationToken, Task<Publisher>> get,
            Related<SeriesSummary> series) : base(list, get) => _series = series;

        public Task<Page<SeriesSummary>> ListSeriesAsync(
            int publisherId, int page = 1, CancellationToken cancellationToken = default) =>
            _series.ListAsync(publisherId, page, cancellationToken);

        public IAsyncEnumerable<SeriesSummary> StreamSeries(
            int publisherId, CancellationToken cancellationToken = default) =>
            _series.Stream(publisherId, cancellationToken);
    }
}
