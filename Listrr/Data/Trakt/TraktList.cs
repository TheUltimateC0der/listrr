using Listrr.Data.Trakt.Filters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Listrr.Data.Trakt
{

    public enum ListType
    {
        Movie,
        Show
    }

    public enum ScanState
    {
        Scheduled,
        Updating,
        None
    }

    public enum ListContentType
    {
        Filters,
        Names,
        Imdb,
        Tvdb,
        Tmdb
    }

    public class TraktList
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public uint Id { get; set; }

        public string Name { get; set; }

        public ListType Type { get; set; }

        public ListContentType ContentType { get; set; }

        public string Slug { get; set; }

        public ScanState ScanState { get; set; }

        public bool Process { get; set; }

        public int? Items { get; set; }

        public int? Likes { get; set; }

        public string ItemList { get; set; }

        public DateTime LastProcessed { get; set; }

        public User Owner { get; set; }


        #region Basic Filter

        public string Query { get; set; }

        public bool SearchByAlias { get; set; }
        public bool SearchByBiography { get; set; }
        public bool SearchByDescription { get; set; }
        public bool SearchByName { get; set; }
        public bool SearchByOverview { get; set; }
        public bool SearchByPeople { get; set; }
        public bool SearchByTitle { get; set; }
        public bool SearchByTranslations { get; set; }
        public bool SearchByTagline { get; set; }

        #endregion

        #region Common Filter

        public YearsCommonFilter Filter_Years { get; set; }

        public RuntimesCommonFilter Filter_Runtimes { get; set; }

        public RatingsCommonFilter Filter_Ratings_Trakt { get; set; }

        public RatingsCommonFilter Filter_Ratings_IMDb { get; set; }

        public LanguagesCommonFilter Filter_Languages { get; set; }

        public GenresCommonFilter Filter_Genres { get; set; }

        public CountriesCommonFilter Filter_Countries { get; set; }

        public TranslationsBasicFilter Filter_Translations { get; set; }

        #endregion

        #region Exclusion Common Filter

        public LanguagesCommonFilter ExclusionFilter_Languages { get; set; }

        public GenresCommonFilter ExclusionFilter_Genres { get; set; }

        public CountriesCommonFilter ExclusionFilter_Countries { get; set; }

        public TranslationsBasicFilter ExclusionFilter_Translations { get; set; }

        public string[] ExclusionFilter_Keywords { get; set; }

        #endregion


        #region Movie Filter

        public CertificationsMovieFilter Filter_Certifications_Movie { get; set; }

        #endregion

        #region Exclusion Movie Filter

        public CertificationsMovieFilter ExclusionFilter_Certifications_Movie { get; set; }

        #endregion


        #region Show Filter

        public CertificationsShowFilter Filter_Certifications_Show { get; set; }

        public NetworksShowFilter Filter_Networks { get; set; }

        public StatusShowFilter Filter_Status { get; set; }

        #endregion

        #region Exclusion Show Filter

        public CertificationsShowFilter ExclusionFilter_Certifications_Show { get; set; }

        public NetworksShowFilter ExclusionFilter_Networks { get; set; }

        public StatusShowFilter ExclusionFilter_Status { get; set; }

        #endregion



        public string GetDescriptionText()
        {
            string result;

            switch (ContentType)
            {
                case ListContentType.Filters:
                    result = "Normal Filters:\r\n";
                    result += "Content Type: Filters\r\n";

                    if (Filter_Certifications_Movie?.Certifications != null)
                        result += $"Certifications: {string.Join(", ", Filter_Certifications_Movie?.Certifications)}\r\n";

                    if (Filter_Certifications_Show?.Certifications != null)
                        result += $"Certifications: {string.Join(", ", Filter_Certifications_Show?.Certifications)}\r\n";

                    if (Filter_Countries?.Languages != null)
                        result += $"Countries: {string.Join(", ", Filter_Countries?.Languages)}\r\n";

                    if (Filter_Genres?.Genres != null)
                        result += $"Genres: {string.Join(", ", Filter_Genres?.Genres)}\r\n";

                    if (Filter_Languages?.Languages != null)
                        result += $"Languages: {string.Join(", ", Filter_Languages?.Languages)}\r\n";

                    if (Filter_Networks?.Networks != null)
                        result += $"Networks: {string.Join(", ", Filter_Networks?.Networks)}\r\n";

                    if (Filter_Ratings_Trakt != null)
                    {
                        result += $"Trakt ratings: Between {Filter_Ratings_Trakt?.From} and {Filter_Ratings_Trakt?.To} with at least {Filter_Ratings_Trakt?.Votes}\r\n";
                    }

                    if (Filter_Ratings_IMDb != null)
                    {
                        result += $"IMDb ratings: Between {Filter_Ratings_IMDb?.From} and {Filter_Ratings_IMDb?.To} with at least {Filter_Ratings_IMDb?.Votes}\r\n";
                    }

                    if (Filter_Runtimes != null)
                    {
                        result += $"Rumtime: Between {Filter_Runtimes?.From} and {Filter_Runtimes?.To} minutes\r\n";
                    }

                    if (Filter_Status?.Status != null)
                        result += $"States: {string.Join(", ", Filter_Status?.Status)}\r\n";

                    if (Filter_Translations?.Translations != null)
                        result += $"Translations: {string.Join(", ", Filter_Translations?.Translations)}\r\n";

                    if (Filter_Years != null)
                    {
                        result += $"Years: Between {Filter_Years?.From} and {Filter_Years?.To}\r\n";
                    }

                    if (ExclusionFilter_Certifications_Movie?.Certifications != null ||
                        ExclusionFilter_Certifications_Show?.Certifications != null ||
                        ExclusionFilter_Countries?.Languages != null ||
                        ExclusionFilter_Genres?.Genres != null ||
                        ExclusionFilter_Languages?.Languages != null ||
                        ExclusionFilter_Networks?.Networks != null ||
                        ExclusionFilter_Status?.Status != null ||
                        ExclusionFilter_Translations?.Translations != null)
                        result += "\r\n\r\n\r\nExclusion filters:\r\n";


                    if (ExclusionFilter_Certifications_Movie?.Certifications != null)
                        result += $"Certifications: {string.Join(", ", ExclusionFilter_Certifications_Movie?.Certifications)}\r\n";

                    if (ExclusionFilter_Certifications_Show?.Certifications != null)
                        result += $"Certifications: {string.Join(", ", ExclusionFilter_Certifications_Show?.Certifications)}\r\n";

                    if (ExclusionFilter_Countries?.Languages != null)
                        result += $"Countries: {string.Join(", ", ExclusionFilter_Countries?.Languages)}\r\n";

                    if (ExclusionFilter_Genres?.Genres != null)
                        result += $"Genres: {string.Join(", ", ExclusionFilter_Genres?.Genres)}\r\n";

                    if (ExclusionFilter_Languages?.Languages != null)
                        result += $"Languages: {string.Join(", ", ExclusionFilter_Languages?.Languages)}\r\n";

                    if (ExclusionFilter_Networks?.Networks != null)
                        result += $"Networks: {string.Join(", ", ExclusionFilter_Networks?.Networks)}\r\n";

                    if (ExclusionFilter_Status?.Status != null)
                        result += $"States: {string.Join(", ", ExclusionFilter_Status?.Status)}\r\n";

                    if (ExclusionFilter_Translations?.Translations != null)
                        result += $"Translations: {string.Join(", ", ExclusionFilter_Translations?.Translations)}\r\n";


                    break;
                case ListContentType.Names:
                    result = "Content Type: Names";

                    break;
                case ListContentType.Imdb:
                    result = "Content Type: IMDb";

                    break;
                case ListContentType.Tvdb:
                    result = "Content Type: TVDb";

                    break;
                case ListContentType.Tmdb:
                    result = "Content Type: TMDb";

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

    }

    public class TraktListConfiguration : IEntityTypeConfiguration<TraktList>
    {
        public void Configure(EntityTypeBuilder<TraktList> builder)
        {
            builder
                .HasOne<User>(x => x.Owner)
                .WithMany(x => x.TraktLists)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.ScanState)
                .HasDefaultValue(ScanState.None);

            builder
                .Property(x => x.Filter_Countries)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CountriesCommonFilter>(x)
                );

            builder
                .Property(x => x.Filter_Certifications_Movie)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CertificationsMovieFilter>(x)
                );

            builder
                .Property(x => x.Filter_Certifications_Show)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CertificationsShowFilter>(x)
                );

            builder
                .Property(x => x.Filter_Networks)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<NetworksShowFilter>(x)
                );

            builder
                .Property(x => x.Filter_Status)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<StatusShowFilter>(x)
                );

            builder
                .Property(x => x.Filter_Genres)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<GenresCommonFilter>(x)
                );

            builder
                .Property(x => x.Filter_Languages)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<LanguagesCommonFilter>(x)
                );

            builder
                .Property(x => x.Filter_Ratings_Trakt)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<RatingsCommonFilter>(x)
                );

            builder
                .Property(x => x.Filter_Ratings_IMDb)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<RatingsCommonFilter>(x)
                );

            builder
                .Property(x => x.Filter_Runtimes)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<RuntimesCommonFilter>(x)
                );

            builder
                .Property(x => x.Filter_Years)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<YearsCommonFilter>(x)
                );

            builder
                .Property(x => x.Filter_Translations)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<TranslationsBasicFilter>(x)
                );
            // Reverse filters
            builder
                .Property(x => x.ExclusionFilter_Certifications_Movie)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CertificationsMovieFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Certifications_Show)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CertificationsShowFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Countries)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CountriesCommonFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Genres)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<GenresCommonFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Languages)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<LanguagesCommonFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Networks)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<NetworksShowFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Status)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<StatusShowFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Translations)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<TranslationsBasicFilter>(x)
                );

            builder
                .Property(x => x.ExclusionFilter_Keywords)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<string[]>(x)
                );
        }
    }
}