using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Listrr.API.Trakt.Models.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Listrr.Models
{
    public class CreateMovieListViewModel
    {
        [Required]
        [Display(Name = "List Name", Prompt = "List Name")]
        public string Name { get; set; }

        [Display(Name = "Search query", Prompt = "Search query")]
        public string Query { get; set; }

        [Display(Name = "Search by title", Prompt = "Search by title")]
        public bool SearchByTitle { get; set; }
        [Display(Name = "Search by tagline", Prompt = "Search by tagline")]
        public bool SearchByTagline { get; set; }
        [Display(Name = "Search by overview", Prompt = "Search by overview")]
        public bool SearchByOverview { get; set; }
        [Display(Name = "Search by people", Prompt = "Search by people")]
        public bool SearchByPeople { get; set; }
        [Display(Name = "Search by translations", Prompt = "Search by translations")]
        public bool SearchByTranslations { get; set; }
        [Display(Name = "Search by alias", Prompt = "Search by alias")]
        public bool SearchByAliases { get; set; }
        [Display(Name = "Search by name", Prompt = "Search by name")]
        public bool SearchByName { get; set; }
        [Display(Name = "Search by biography", Prompt = "Search by biography")]
        public bool SearchByBiography { get; set; }
        [Display(Name = "Search by description", Prompt = "Search by description")]
        public bool SearchByDescription { get; set; }

        #region Basic Filter

        [Display(Name = "Movie title", Prompt = "Movie title")]
        public string Filter_Title { get; set; }

        [Display(Name = "Movie tagline", Prompt = "Movie tagline")]
        public string Filter_Tagline { get; set; }

        [Display(Name = "Movie overview", Prompt = "Movie overview")]
        public string Filter_Overview { get; set; }

        [Display(Name = "Movie people", Prompt = "Movie people")]
        public string Filter_People { get; set; }

        [Display(Name = "Movie translation", Prompt = "de,en,ru")]
        public IEnumerable<string> Filter_Translations { get; set; }

        public MultiSelectList Translations { get; set; }

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
        public IEnumerable<string> Filter_Languages { get; set; }
        public MultiSelectList Languages { get; set; }

        [Display(Name = "Movie genres", Prompt = "action,adventure")]
        public IEnumerable<string> Filter_Genres { get; set; }
        public MultiSelectList Genres { get; set; }

        [Display(Name = "Movie countries", Prompt = "uk,us,de,ru")]
        public IEnumerable<string> Filter_Countries { get; set; }
        public MultiSelectList Countries { get; set; }

        #endregion

        #region Movie Filter

        [Display(Name = "Movie certification", Prompt = "r,pg-13")]
        public IEnumerable<string>  Filter_Certifications { get; set; }

        public MultiSelectList Certifications { get; set; }

        #endregion

    }
}