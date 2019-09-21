using System;
using System.ComponentModel.DataAnnotations.Schema;
using Listrr.API.Trakt.Models.Filters;
using Microsoft.AspNetCore.Identity;
using TraktNet.Enums;

namespace Listrr.Data.Trakt
{

    public enum ListType
    {
        Movie,
        Show
    }

    public class TraktList
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public uint Id { get; set; }

        public string Name { get; set; }

        public ListType Type { get; set; }

        public string Slug { get; set; }

        public bool Process { get; set; }

        public int? Items { get; set; }

        public int? Likes { get; set; }

        public DateTime LastProcessed { get; set; }

        public IdentityUser Owner { get; set; }


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

        public TranslationsBasicFilter Filter_Translations { get; set; }

        #endregion

        #region Common Filter

        public YearsCommonFilter Filter_Years { get; set; }

        public RuntimesCommonFilter Filter_Runtimes { get; set; }

        public RatingsCommonFilter Filter_Ratings { get; set; }

        public LanguagesCommonFilter Filter_Languages { get; set; }

        public GenresCommonFilter Filter_Genres { get; set; }

        public CountriesCommonFilter Filter_Countries { get; set; }

        #endregion

        #region Movie Filter

        public CertificationsMovieFilter Filter_Certifications_Movie { get; set; }

        #endregion

        #region Show Filter

        public CertificationsShowFilter Filter_Certifications_Show { get; set; }

        public NetworksShowFilter Filter_Networks { get; set; }

        public StatusShowFilter Filter_Status { get; set; }

        #endregion

    }
}