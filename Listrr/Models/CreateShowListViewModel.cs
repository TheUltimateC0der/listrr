using Microsoft.AspNetCore.Mvc.Rendering;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Listrr.Models
{
    public class CreateShowListViewModel : CreateListViewModel
    {

        #region Common filter

        [Display(Name = "Genres", Prompt = "action,adventure")]
        public IEnumerable<string> Filter_Genres { get; set; }
        public MultiSelectList Genres { get; set; }

        #endregion

        #region Exclusion Common filter

        [Display(Name = "Genres", Prompt = "action,adventure")]
        public IEnumerable<string> ExclusionFilter_Genres { get; set; }
        public MultiSelectList ExclusionGenres { get; set; }

        #endregion


        #region Show filter

        [Display(Name = "Certifications", Prompt = "r,pg-13")]
        public IEnumerable<string> Filter_Certifications { get; set; }
        public MultiSelectList Certifications { get; set; }


        [Display(Name = "Networks", Prompt = "netflix,amazon")]
        public IEnumerable<string> Filter_Networks { get; set; }
        public MultiSelectList Networks { get; set; }


        [Display(Name = "Status", Prompt = "planned,in production")]
        public IEnumerable<string> Filter_Status { get; set; }
        public MultiSelectList Status { get; set; }

        #endregion

        #region Exclusion Show filter

        [Display(Name = "Certifications", Prompt = "r,pg-13")]
        public IEnumerable<string> ExclusionFilter_Certifications { get; set; }
        public MultiSelectList ExclusionCertifications { get; set; }


        [Display(Name = "Networks", Prompt = "netflix,amazon")]
        public IEnumerable<string> ExclusionFilter_Networks { get; set; }
        public MultiSelectList ExclusionNetworks { get; set; }


        [Display(Name = "Status", Prompt = "planned,in production")]
        public IEnumerable<string> ExclusionFilter_Status { get; set; }
        public MultiSelectList ExclusionStatus { get; set; }

        #endregion

    }
}