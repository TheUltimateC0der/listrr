using Microsoft.AspNetCore.Mvc.Rendering;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Listrr.Models
{
    public class CreateMovieListViewModel : CreateListViewModel
    {
        #region Common filter

        [Display(Name = "Genres", Prompt = "action,adventure")]
        public IEnumerable<string> Filter_Genres { get; set; }
        public MultiSelectList Genres { get; set; }

        #endregion

        #region Reverse Common filter

        [Display(Name = "Genres", Prompt = "action,adventure")]
        public IEnumerable<string> ReverseFilter_Genres { get; set; }
        public MultiSelectList ReverseGenres { get; set; }

        #endregion


        #region Movie filter

        [Display(Name = "Certifications", Prompt = "r,pg-13")]
        public IEnumerable<string> Filter_Certifications { get; set; }

        public MultiSelectList Certifications { get; set; }

        #endregion

        #region Reverse Movie filter

        [Display(Name = "Certifications", Prompt = "r,pg-13")]
        public IEnumerable<string> ReverseFilter_Certifications { get; set; }
        public MultiSelectList ReverseCertifications { get; set; }

        #endregion

    }
}