using System;
using System.Collections.Generic;

namespace Metron.Api
{
    /// <summary>An id and name pair that points at another resource.</summary>
    public sealed class NamedRef
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class PublisherSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Founded { get; set; }
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public IReadOnlyList<string> AltNames { get; set; } = Array.Empty<string>();
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class ImprintSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Imprint
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Founded { get; set; }
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public NamedRef? Publisher { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class SeriesSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Volume { get; set; }
        public int? YearBegan { get; set; }
        public int? YearEnd { get; set; }
        public int IssueCount { get; set; }
        public NamedRef? Publisher { get; set; }
        public NamedRef? SeriesType { get; set; }
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Series
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SortName { get; set; }
        public int? Volume { get; set; }
        public int? YearBegan { get; set; }
        public int? YearEnd { get; set; }
        public int IssueCount { get; set; }
        public string? Status { get; set; }
        public string? Desc { get; set; }
        public NamedRef? Publisher { get; set; }
        public NamedRef? Imprint { get; set; }
        public NamedRef? SeriesType { get; set; }
        public IReadOnlyList<string> AltNames { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> Genres { get; set; } = Array.Empty<string>();
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class IssueSummary
    {
        public int Id { get; set; }

        /// <summary>Display name such as "Batman (2016) #7".</summary>
        public string Name { get; set; } = string.Empty;
        public string? Number { get; set; }
        public NamedRef? Series { get; set; }
        public DateTime? CoverDate { get; set; }
        public DateTime? StoreDate { get; set; }
        public string? Image { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Issue
    {
        public int Id { get; set; }

        /// <summary>Series name and issue number, such as "Batman #7".</summary>
        public string Name { get; set; } = string.Empty;
        public string? Number { get; set; }
        public string? AltNumber { get; set; }
        public IReadOnlyList<string> Titles { get; set; } = Array.Empty<string>();
        public NamedRef? Series { get; set; }
        public NamedRef? Publisher { get; set; }
        public NamedRef? Imprint { get; set; }
        public DateTime? CoverDate { get; set; }
        public DateTime? StoreDate { get; set; }
        public DateTime? FocDate { get; set; }
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public string? Isbn { get; set; }
        public string? Upc { get; set; }
        public string? Sku { get; set; }
        public int? PageCount { get; set; }
        public IReadOnlyList<Credit> Credits { get; set; } = Array.Empty<Credit>();
        public IReadOnlyList<NamedRef> Characters { get; set; } = Array.Empty<NamedRef>();
        public IReadOnlyList<NamedRef> Teams { get; set; } = Array.Empty<NamedRef>();
        public IReadOnlyList<NamedRef> Arcs { get; set; } = Array.Empty<NamedRef>();
        public IReadOnlyList<NamedRef> Universes { get; set; } = Array.Empty<NamedRef>();
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    /// <summary>A creator and the roles they held on an issue.</summary>
    public sealed class Credit
    {
        public int CreatorId { get; set; }
        public string Creator { get; set; } = string.Empty;
        public IReadOnlyList<Role> Roles { get; set; } = Array.Empty<Role>();
    }

    public sealed class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class SeriesType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class CharacterSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public IReadOnlyList<string> Aliases { get; set; } = Array.Empty<string>();
        public IReadOnlyList<NamedRef> Creators { get; set; } = Array.Empty<NamedRef>();
        public IReadOnlyList<NamedRef> Teams { get; set; } = Array.Empty<NamedRef>();
        public IReadOnlyList<NamedRef> Universes { get; set; } = Array.Empty<NamedRef>();
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class CreatorSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Creator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public DateTime? Birth { get; set; }
        public DateTime? Death { get; set; }
        public IReadOnlyList<string> Aliases { get; set; } = Array.Empty<string>();
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class TeamSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public IReadOnlyList<NamedRef> Creators { get; set; } = Array.Empty<NamedRef>();
        public IReadOnlyList<NamedRef> Universes { get; set; } = Array.Empty<NamedRef>();
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class ArcSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Arc
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public int? CvId { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class UniverseSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Modified { get; set; }
    }

    public sealed class Universe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public NamedRef? Publisher { get; set; }
        public int? GcdId { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
