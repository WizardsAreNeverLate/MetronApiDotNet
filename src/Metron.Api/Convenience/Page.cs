using System;
using System.Collections.Generic;

namespace Metron.Api
{
    /// <summary>One page of results from a Metron list endpoint.</summary>
    public sealed class Page<T>
    {
        public Page(IReadOnlyList<T> items, int totalCount, int pageNumber, bool hasNext)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = totalCount;
            PageNumber = pageNumber;
            HasNext = hasNext;
        }

        /// <summary>The entries on this page.</summary>
        public IReadOnlyList<T> Items { get; }

        /// <summary>Number of entries across all pages that match the filter.</summary>
        public int TotalCount { get; }

        /// <summary>One-based number of this page.</summary>
        public int PageNumber { get; }

        /// <summary>True when the server reported a following page.</summary>
        public bool HasNext { get; }
    }
}
