using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Listrr.API.Trakt.Models.Filters;

namespace Listrr.Models
{
    public class CreateMovieListViewModel
    {
        [Required]
        [Display(Name = "List Name", Prompt = "List Name")]
        public string Name { get; set; }

        #region Basic Filter
        [Display(Name = "Movie title", Prompt = "Movie title")]
        public string Filter_Title { get; set; }

        [Display(Name = "Movie tagline", Prompt = "Movie tagline")]
        public string Filter_Tagline { get; set; }

        [Display(Name = "Movie overview", Prompt = "Movie overview")]
        public string Filter_Overview { get; set; }

        [Display(Name = "Movie people", Prompt = "Movie people")]
        public string Filter_People { get; set; }

        [Display(Name = "Movie translation", Prompt = "Movie translation")]
        public string Filter_Translations { get; set; }

        [Display(Name = "Movie aliases", Prompt = "Movie aliases")]
        public string Filter_Aliases { get; set; }

        #endregion

        #region Common Filter

        [Display(Name = "Release year", Prompt = "2010 or 1990-2000")]
        public YearsCommonFilter Filter_Years { get; set; }

        [Display(Name = "Movie runtime", Prompt = "30-200")]
        public RuntimesCommonFilter Filter_Runtimes { get; set; }

        [Display(Name = "Movie rating", Prompt = "0-10 or 5")]
        public RatingsCommonFilter Filter_Ratings { get; set; }

        [Display(Name = "Movie language", Prompt = "en,de,ru")]
        public LanguagesCommonFilter Filter_Languages { get; set; }

        [Display(Name = "Movie genres", Prompt = "action,adventure")]
        public GenresCommonFilter Filter_Genres { get; set; }

        [Display(Name = "Movie countries", Prompt = "uk,us,de,ru")]
        public CountriesCommonFilter Filter_Countries { get; set; }

        #endregion

        #region Movie Filter

        [Display(Name = "Movie certification", Prompt = "r,pg-13")]
        public CertificationsMovieFilter Filter_Certifications { get; set; }

        #endregion

    }
}