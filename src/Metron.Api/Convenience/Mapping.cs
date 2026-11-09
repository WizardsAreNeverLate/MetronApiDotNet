using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kiota.Abstractions;
using K = Metron.Api.Client.Models;

namespace Metron.Api
{
    internal static class Mapping
    {
        public static PublisherSummary Map(K.PublisherList x) => new PublisherSummary
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Modified = x.Modified
        };

        public static Publisher Map(K.PublisherRead x) => new Publisher
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Founded = x.Founded,
            Desc = x.Desc,
            Image = x.Image,
            CvId = x.CvId,
            GcdId = x.GcdId,
            AltNames = Strings(x.AltNames),
            Modified = x.Modified
        };

        public static ImprintSummary Map(K.ImprintList x) => new ImprintSummary
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Modified = x.Modified
        };

        public static Imprint Map(K.ImprintRead x) => new Imprint
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Founded = x.Founded,
            Desc = x.Desc,
            Image = x.Image,
            CvId = x.CvId,
            GcdId = x.GcdId,
            Publisher = Ref(x.Publisher?.Id, x.Publisher?.Name),
            Modified = x.Modified
        };

        public static SeriesSummary Map(K.SeriesList x) => new SeriesSummary
        {
            Id = x.Id ?? 0,
            Name = x.Series ?? string.Empty,
            Volume = x.Volume,
            YearBegan = x.YearBegan,
            YearEnd = x.YearEnd,
            IssueCount = x.IssueCount ?? 0,
            Publisher = Ref(x.Publisher?.Id, x.Publisher?.Name),
            SeriesType = Ref(x.SeriesType?.Id, x.SeriesType?.Name),
            CvId = x.CvId,
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        public static Series Map(K.SeriesRead x) => new Series
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            SortName = x.SortName,
            Volume = x.Volume,
            YearBegan = x.YearBegan,
            YearEnd = x.YearEnd,
            IssueCount = x.IssueCount ?? 0,
            Status = x.Status,
            Desc = x.Desc,
            Publisher = Ref(x.Publisher?.Id, x.Publisher?.Name),
            Imprint = Ref(x.Imprint?.Id, x.Imprint?.Name),
            SeriesType = Ref(x.SeriesType?.Id, x.SeriesType?.Name),
            AltNames = Strings(x.AltNames),
            Genres = (x.Genres ?? new List<K.Genre>()).Select(g => g.Name ?? string.Empty).ToList(),
            CvId = x.CvId,
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        public static IssueSummary Map(K.IssueList x) => new IssueSummary
        {
            Id = x.Id ?? 0,
            Name = x.Issue ?? string.Empty,
            Number = x.Number,
            Series = Ref(x.Series?.Id, x.Series?.Name),
            CoverDate = Day(x.CoverDate),
            StoreDate = Day(x.StoreDate),
            Image = x.Image,
            Modified = x.Modified
        };

        public static Issue Map(K.IssueRead x) => new Issue
        {
            Id = x.Id ?? 0,
            Name = string.Join(" #", new[] { x.Series?.Name, x.Number }.Where(s => !string.IsNullOrEmpty(s))),
            Number = x.Number,
            AltNumber = x.AltNumber,
            Titles = Strings(x.Name),
            Series = Ref(x.Series?.Id, x.Series?.Name),
            Publisher = Ref(x.Publisher?.Id, x.Publisher?.Name),
            Imprint = Ref(x.Imprint?.Id, x.Imprint?.Name),
            CoverDate = Day(x.CoverDate),
            StoreDate = Day(x.StoreDate),
            FocDate = Day(x.FocDate),
            Desc = x.Desc,
            Image = x.Image,
            Isbn = x.Isbn,
            Upc = x.Upc,
            Sku = x.Sku,
            PageCount = x.Page,
            Credits = (x.Credits ?? new List<K.CreditRead>()).Select(Map).ToList(),
            Characters = Refs(x.Characters, c => Ref(c.Id, c.Name)),
            Teams = Refs(x.Teams, t => Ref(t.Id, t.Name)),
            Arcs = Refs(x.Arcs, a => Ref(a.Id, a.Name)),
            Universes = Refs(x.Universes, u => Ref(u.Id, u.Name)),
            CvId = x.CvId,
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        public static Credit Map(K.CreditRead x) => new Credit
        {
            CreatorId = x.Id ?? 0,
            Creator = x.Creator ?? string.Empty,
            Roles = (x.Role ?? new List<K.Role>()).Select(Map).ToList()
        };

        public static Role Map(K.Role x) => new Role { Id = x.Id ?? 0, Name = x.Name ?? string.Empty };

        public static SeriesType Map(K.SeriesType x) => new SeriesType { Id = x.Id ?? 0, Name = x.Name ?? string.Empty };

        public static CharacterSummary Map(K.CharacterList x) => new CharacterSummary
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Modified = x.Modified
        };

        public static Character Map(K.CharacterRead x) => new Character
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Desc = x.Desc,
            Image = x.Image,
            Aliases = Strings(x.Alias),
            Creators = Refs(x.Creators, c => Ref(c.Id, c.Name)),
            Teams = Refs(x.Teams, t => Ref(t.Id, t.Name)),
            Universes = Refs(x.Universes, u => Ref(u.Id, u.Name)),
            CvId = x.CvId,
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        public static CreatorSummary Map(K.CreatorList x) => new CreatorSummary
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Modified = x.Modified
        };

        public static Creator Map(K.CreatorRead x) => new Creator
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Desc = x.Desc,
            Image = x.Image,
            Birth = Day(x.Birth),
            Death = Day(x.Death),
            Aliases = Strings(x.Alias),
            CvId = x.CvId,
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        public static TeamSummary Map(K.TeamList x) => new TeamSummary
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Modified = x.Modified
        };

        public static Team Map(K.TeamRead x) => new Team
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Desc = x.Desc,
            Image = x.Image,
            Creators = Refs(x.Creators, c => Ref(c.Id, c.Name)),
            Universes = Refs(x.Universes, u => Ref(u.Id, u.Name)),
            CvId = x.CvId,
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        public static ArcSummary Map(K.ArcList x) => new ArcSummary
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Modified = x.Modified
        };

        public static Arc Map(K.ArcRead x) => new Arc
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Desc = x.Desc,
            Image = x.Image,
            CvId = x.CvId,
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        public static UniverseSummary Map(K.UniverseList x) => new UniverseSummary
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Modified = x.Modified
        };

        public static Universe Map(K.UniverseRead x) => new Universe
        {
            Id = x.Id ?? 0,
            Name = x.Name ?? string.Empty,
            Designation = x.Designation,
            Desc = x.Desc,
            Image = x.Image,
            Publisher = Ref(x.Publisher?.Id, x.Publisher?.Name),
            GcdId = x.GcdId,
            Modified = x.Modified
        };

        private static NamedRef? Ref(int? id, string? name) =>
            id is null && name is null ? null : new NamedRef { Id = id ?? 0, Name = name ?? string.Empty };

        private static IReadOnlyList<NamedRef> Refs<T>(List<T>? source, Func<T, NamedRef?> map) =>
            (source ?? new List<T>()).Select(map).Where(r => r != null).Select(r => r!).ToList();

        private static IReadOnlyList<string> Strings(List<string>? source) =>
            source == null ? Array.Empty<string>() : source.ToList();

        private static DateTime? Day(Date? date) => date?.DateTime;
    }
}
