using System;
using System.ComponentModel.DataAnnotations.Schema;

using Listrr.API.Trakt.Models.Filters;

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

        public RatingsCommonFilter Filter_Ratings { get; set; }

        public LanguagesCommonFilter Filter_Languages { get; set; }

        public GenresCommonFilter Filter_Genres { get; set; }

        public CountriesCommonFilter Filter_Countries { get; set; }

        public TranslationsBasicFilter Filter_Translations { get; set; }

        #endregion

        #region Reverse Common Filter

        public LanguagesCommonFilter ReverseFilter_Languages { get; set; }

        public GenresCommonFilter ReverseFilter_Genres { get; set; }

        public CountriesCommonFilter ReverseFilter_Countries { get; set; }

        public TranslationsBasicFilter ReverseFilter_Translations { get; set; }

        #endregion


        #region Movie Filter

        public CertificationsMovieFilter Filter_Certifications_Movie { get; set; }

        #endregion

        #region Reverse Movie Filter

        public CertificationsMovieFilter ReverseFilter_Certifications_Movie { get; set; }

        #endregion


        #region Show Filter

        public CertificationsShowFilter Filter_Certifications_Show { get; set; }

        public NetworksShowFilter Filter_Networks { get; set; }

        public StatusShowFilter Filter_Status { get; set; }

        #endregion

        #region Revsere Show Filter

        public CertificationsShowFilter ReverseFilter_Certifications_Show { get; set; }

        public NetworksShowFilter ReverseFilter_Networks { get; set; }

        public StatusShowFilter ReverseFilter_Status { get; set; }

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

                    if (Filter_Ratings != null)
                        result += $"Min rating: {Filter_Ratings?.From}\r\n";
                    if (Filter_Ratings != null)
                        result += $"Max rating: {Filter_Ratings?.To}\r\n";
                    if (Filter_Ratings != null)
                        result += $"Min votes: {Filter_Ratings?.Votes}\r\n";

                    if (Filter_Runtimes != null)
                        result += $"Min runtime: {Filter_Runtimes?.From}\r\n";
                    if (Filter_Runtimes != null)
                        result += $"Max runtime: {Filter_Runtimes?.To}\r\n";

                    if (Filter_Status?.Status != null)
                        result += $"States: {string.Join(", ", Filter_Status?.Status)}\r\n";

                    if (Filter_Translations?.Translations != null)
                        result += $"Translations: {string.Join(", ", Filter_Translations?.Translations)}\r\n";

                    if (Filter_Years != null)
                        result += $"Min year: {Filter_Years?.From}\r\n";
                    if (Filter_Years != null)
                        result += $"Max year: {Filter_Years?.To}\r\n";


                    if (ReverseFilter_Certifications_Movie?.Certifications != null ||
                        ReverseFilter_Certifications_Show?.Certifications != null ||
                        ReverseFilter_Countries?.Languages != null ||
                        ReverseFilter_Genres?.Genres != null ||
                        ReverseFilter_Languages?.Languages != null ||
                        ReverseFilter_Networks?.Networks != null ||
                        ReverseFilter_Status?.Status != null ||
                        ReverseFilter_Translations?.Translations != null)
                        result += "\r\n\r\n\r\nReverse filters:\r\n";


                    if (ReverseFilter_Certifications_Movie?.Certifications != null)
                        result += $"Certifications: {string.Join(", ", ReverseFilter_Certifications_Movie?.Certifications)}\r\n";

                    if (ReverseFilter_Certifications_Show?.Certifications != null)
                        result += $"Certifications: {string.Join(", ", ReverseFilter_Certifications_Show?.Certifications)}\r\n";

                    if (ReverseFilter_Countries?.Languages != null)
                        result += $"Countries: {string.Join(", ", ReverseFilter_Countries?.Languages)}\r\n";

                    if (ReverseFilter_Genres?.Genres != null)
                        result += $"Genres: {string.Join(", ", ReverseFilter_Genres?.Genres)}\r\n";

                    if (ReverseFilter_Languages?.Languages != null)
                        result += $"Languages: {string.Join(", ", ReverseFilter_Languages?.Languages)}\r\n";

                    if (ReverseFilter_Networks?.Networks != null)
                        result += $"Networks: {string.Join(", ", ReverseFilter_Networks?.Networks)}\r\n";

                    if (ReverseFilter_Status?.Status != null)
                        result += $"States: {string.Join(", ", ReverseFilter_Status?.Status)}\r\n";

                    if (ReverseFilter_Translations?.Translations != null)
                        result += $"Translations: {string.Join(", ", ReverseFilter_Translations?.Translations)}\r\n";


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
}