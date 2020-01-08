using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Listrr.API.Trakt.Models.Filters;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Listrr.Models
{
    public class CreateListViewModel
    {
        [Required]
        [Display(Name = "List Name", Prompt = "List Name")]
        public string Name { get; set; }

        [Display(Name = "Search query", Prompt = "Search query")]
        public string Query { get; set; }

        [Display(Name = "Search by alias", Prompt = "Search by alias")]
        public bool SearchByAlias { get; set; }

        [Display(Name = "Search by biography", Prompt = "Search by biography")]
        public bool SearchByBiography { get; set; }

        [Display(Name = "Search by description", Prompt = "Search by description")]
        public bool SearchByDescription { get; set; }


        [Display(Name = "Search by name", Prompt = "Search by name")]
        public bool SearchByName { get; set; }

        [Display(Name = "Search by overview", Prompt = "Search by overview")]
        public bool SearchByOverview { get; set; }

        [Display(Name = "Search by people", Prompt = "Search by people")]
        public bool SearchByPeople { get; set; }


        [Display(Name = "Search by tagline", Prompt = "Search by tagline")]
        public bool SearchByTagline { get; set; }

        [Display(Name = "Search by title", Prompt = "Search by title")]
        public bool SearchByTitle { get; set; }
        
        [Display(Name = "Search by translations", Prompt = "Search by translations")]
        public bool SearchByTranslations { get; set; }

        [Display(Name = "Minimum amount of votes", Prompt = "Minimum amount of votes")]
        public int MinVotes { get; set; }

        #region Common Filter

        [Display(Name = "Movie translation", Prompt = "de,en,ru")]
        public IEnumerable<string> Filter_Translations { get; set; }
        public MultiSelectList Translations { get; set; }

        [Display(Name = "Release year", Prompt = "2010 or 1990-2000")]
        public YearsCommonFilter Filter_Years { get; set; }

        [Display(Name = "Runtime", Prompt = "30-200")]
        public RuntimesCommonFilter Filter_Runtimes { get; set; }

        [Display(Name = "Rating", Prompt = "0-10 or 5")]
        public RatingsCommonFilter Filter_Ratings { get; set; }

        [Display(Name = "Languages", Prompt = "en,de,ru")]
        public IEnumerable<string> Filter_Languages { get; set; }
        public MultiSelectList Languages { get; set; }

        [Display(Name = "Countries", Prompt = "uk,us,de,ru")]
        public IEnumerable<string> Filter_Countries { get; set; }
        public MultiSelectList Countries { get; set; }

        #endregion

        #region Reverse Common Filter

        [Display(Name = "Languages", Prompt = "en,de,ru")]
        public IEnumerable<string> ReverseFilter_Languages { get; set; }
        public MultiSelectList ReverseLanguages { get; set; }

        [Display(Name = "Countries", Prompt = "uk,us,de,ru")]
        public IEnumerable<string> ReverseFilter_Countries { get; set; }
        public MultiSelectList ReverseCountries { get; set; }

        [Display(Name = "Translations", Prompt = "de,en,ru")]
        public IEnumerable<string> ReverseFilter_Translations { get; set; }
        public MultiSelectList ReverseTranslations { get; set; }

        #endregion

    }
}