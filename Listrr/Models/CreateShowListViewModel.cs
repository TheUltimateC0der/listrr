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

        #region Reverse Common filter

        [Display(Name = "Genres", Prompt = "action,adventure")]
        public IEnumerable<string> ReverseFilter_Genres { get; set; }
        public MultiSelectList ReverseGenres { get; set; }

        #endregion


        #region Show filter

        [Display(Name = "Certifications", Prompt = "r,pg-13")]
        public IEnumerable<string>  Filter_Certifications { get; set; }
        public MultiSelectList Certifications { get; set; }


        [Display(Name = "Networks", Prompt = "netflix,amazon")]
        public IEnumerable<string> Filter_Networks { get; set; }
        public MultiSelectList Networks { get; set; }


        [Display(Name = "Status", Prompt = "planned,in production")]
        public IEnumerable<string> Filter_Status { get; set; }
        public MultiSelectList Status { get; set; }

        #endregion
        
        #region Reverse Show filter

        [Display(Name = "Certifications", Prompt = "r,pg-13")]
        public IEnumerable<string> ReverseFilter_Certifications { get; set; }
        public MultiSelectList ReverseCertifications { get; set; }


        [Display(Name = "Networks", Prompt = "netflix,amazon")]
        public IEnumerable<string> ReverseFilter_Networks { get; set; }
        public MultiSelectList ReverseNetworks { get; set; }


        [Display(Name = "Status", Prompt = "planned,in production")]
        public IEnumerable<string> ReverseFilter_Status { get; set; }
        public MultiSelectList ReverseStatus { get; set; }

        #endregion

    }
}