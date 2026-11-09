using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kiota.Abstractions;
using ArcQ = Metron.Api.Client.Api.Arc.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using CharacterQ = Metron.Api.Client.Api.Character.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using CreatorQ = Metron.Api.Client.Api.Creator.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using ImprintQ = Metron.Api.Client.Api.Imprint.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using IssueQ = Metron.Api.Client.Api.Issue.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using PublisherQ = Metron.Api.Client.Api.Publisher.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using RoleQ = Metron.Api.Client.Api.Role.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using SeriesQ = Metron.Api.Client.Api.Series.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using SeriesTypeQ = Metron.Api.Client.Api.Series_type.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using TeamQ = Metron.Api.Client.Api.Team.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;
using UniverseQ = Metron.Api.Client.Api.Universe.EmptyPathSegmentRequestBuilder.EmptyPathSegmentRequestBuilderGetQueryParameters;

namespace Metron.Api
{
    internal static class QueryMapping
    {
        public static void Apply(PublisherQ q, PublisherFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
            q.AltNames = f.AltNames;
            q.Q = f.Q;
        }

        public static void Apply(SeriesQ q, SeriesFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
            q.AltNames = f.AltNames;
            q.Q = f.Q;
            q.PublisherId = f.PublisherId;
            q.PublisherName = f.PublisherName;
            q.ImprintId = f.ImprintId;
            q.ImprintName = f.ImprintName;
            q.SeriesTypeId = f.SeriesTypeId;
            q.SeriesType = f.SeriesType;
            q.Status = f.Status;
            q.Volume = f.Volume;
            q.YearBegan = f.YearBegan;
            q.YearEnd = f.YearEnd;
            q.CreatorId = f.CreatorId;
            q.TeamId = f.TeamId;
            q.UniverseId = f.UniverseId;
            q.RoleId = Ids(f.RoleIds);
            q.MissingCvId = f.MissingCvId;
            q.MissingGcdId = f.MissingGcdId;
        }

        public static void Apply(IssueQ q, IssueFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
            q.ModifiedGt = f.ModifiedGt;
            q.Number = f.Number;
            q.AltNumber = f.AltNumber;
            q.SeriesId = f.SeriesId;
            q.SeriesName = f.SeriesName;
            q.SeriesAltNames = f.SeriesAltNames;
            q.SeriesQ = f.SeriesQ;
            q.SeriesVolume = f.SeriesVolume;
            q.SeriesYearBegan = f.SeriesYearBegan;
            q.PublisherId = f.PublisherId;
            q.PublisherName = f.PublisherName;
            q.ImprintId = f.ImprintId;
            q.ImprintName = f.ImprintName;
            q.CharacterId = f.CharacterId;
            q.CreatorId = f.CreatorId;
            q.TeamId = f.TeamId;
            q.UniverseId = f.UniverseId;
            q.RoleId = Ids(f.RoleIds);
            q.Rating = f.Rating;
            q.Sku = f.Sku;
            q.Upc = f.Upc;
            q.UpcStartsWith = f.UpcStartsWith;
            q.CoverHash = f.CoverHash;
            q.CoverYear = f.CoverYear;
            q.CoverMonth = f.CoverMonth;
            q.CoverDateRangeAfter = Day(f.CoverDateAfter);
            q.CoverDateRangeBefore = Day(f.CoverDateBefore);
            q.StoreDate = Day(f.StoreDate);
            q.StoreDateRangeAfter = Day(f.StoreDateAfter);
            q.StoreDateRangeBefore = Day(f.StoreDateBefore);
            q.FocDate = Day(f.FocDate);
            q.FocDateRangeAfter = Day(f.FocDateAfter);
            q.FocDateRangeBefore = Day(f.FocDateBefore);
            q.DescIsSolicitation = f.DescIsSolicitation;
            q.MissingCvId = f.MissingCvId;
            q.MissingGcdId = f.MissingGcdId;
        }

        public static void Apply(CharacterQ q, NamedEntityFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
        }

        public static void Apply(CreatorQ q, NamedEntityFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
        }

        public static void Apply(TeamQ q, NamedEntityFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
        }

        public static void Apply(ArcQ q, NamedEntityFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
        }

        public static void Apply(ImprintQ q, NamedEntityFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.CvId = f.CvId;
            q.GcdId = f.GcdId;
        }

        public static void Apply(UniverseQ q, UniverseFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
            q.Designation = f.Designation;
        }

        public static void Apply(RoleQ q, LookupFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
        }

        public static void Apply(SeriesTypeQ q, LookupFilter? f, int page)
        {
            q.Page = page;
            if (f == null) return;
            q.Name = f.Name;
            q.ModifiedGt = f.ModifiedGt;
        }

        private static int?[]? Ids(IReadOnlyList<int>? ids) =>
            ids == null || ids.Count == 0 ? null : ids.Select(i => (int?)i).ToArray();

        private static Date? Day(DateTime? date) => date.HasValue ? new Date(date.Value) : (Date?)null;
    }
}
