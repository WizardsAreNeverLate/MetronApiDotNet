using System;
using System.Collections.Generic;

namespace Metron.Api
{
    /// <summary>Filters shared by every list endpoint that has a name.</summary>
    public class LookupFilter
    {
        /// <summary>Case-insensitive name match.</summary>
        public string? Name { get; set; }

        /// <summary>Only entries modified after this moment.</summary>
        public DateTimeOffset? ModifiedGt { get; set; }
    }

    /// <summary>Filters for resources that carry Comic Vine and Grand Comics Database ids.</summary>
    public class NamedEntityFilter : LookupFilter
    {
        public int? CvId { get; set; }

        public int? GcdId { get; set; }
    }

    public sealed class PublisherFilter : NamedEntityFilter
    {
        public string? AltNames { get; set; }

        /// <summary>Free-text search over name and alternative names.</summary>
        public string? Q { get; set; }
    }

    public sealed class SeriesFilter : NamedEntityFilter
    {
        public string? AltNames { get; set; }

        public string? Q { get; set; }

        public int? PublisherId { get; set; }

        public string? PublisherName { get; set; }

        public int? ImprintId { get; set; }

        public string? ImprintName { get; set; }

        public int? SeriesTypeId { get; set; }

        public string? SeriesType { get; set; }

        /// <summary>Series status id as defined by Metron.</summary>
        public int? Status { get; set; }

        public int? Volume { get; set; }

        public int? YearBegan { get; set; }

        public int? YearEnd { get; set; }

        public int? CreatorId { get; set; }

        public int? TeamId { get; set; }

        public int? UniverseId { get; set; }

        /// <summary>Restrict the creator filter to these role ids.</summary>
        public IReadOnlyList<int>? RoleIds { get; set; }

        public bool? MissingCvId { get; set; }

        public bool? MissingGcdId { get; set; }
    }

    public sealed class IssueFilter
    {
        public int? CvId { get; set; }

        public int? GcdId { get; set; }

        public DateTimeOffset? ModifiedGt { get; set; }

        public string? Number { get; set; }

        public string? AltNumber { get; set; }

        public int? SeriesId { get; set; }

        public string? SeriesName { get; set; }

        public string? SeriesAltNames { get; set; }

        public string? SeriesQ { get; set; }

        public int? SeriesVolume { get; set; }

        public int? SeriesYearBegan { get; set; }

        public int? PublisherId { get; set; }

        public string? PublisherName { get; set; }

        public int? ImprintId { get; set; }

        public string? ImprintName { get; set; }

        public int? CharacterId { get; set; }

        public int? CreatorId { get; set; }

        public int? TeamId { get; set; }

        public int? UniverseId { get; set; }

        /// <summary>Restrict the creator filter to these role ids.</summary>
        public IReadOnlyList<int>? RoleIds { get; set; }

        public string? Rating { get; set; }

        public string? Sku { get; set; }

        public string? Upc { get; set; }

        public string? UpcStartsWith { get; set; }

        public string? CoverHash { get; set; }

        public int? CoverYear { get; set; }

        public int? CoverMonth { get; set; }

        public DateTime? CoverDateAfter { get; set; }

        public DateTime? CoverDateBefore { get; set; }

        public DateTime? StoreDate { get; set; }

        public DateTime? StoreDateAfter { get; set; }

        public DateTime? StoreDateBefore { get; set; }

        public DateTime? FocDate { get; set; }

        public DateTime? FocDateAfter { get; set; }

        public DateTime? FocDateBefore { get; set; }

        public bool? DescIsSolicitation { get; set; }

        public bool? MissingCvId { get; set; }

        public bool? MissingGcdId { get; set; }
    }

    public sealed class UniverseFilter
    {
        public string? Name { get; set; }

        public string? Designation { get; set; }

        public DateTimeOffset? ModifiedGt { get; set; }
    }
}
