using System.ComponentModel.DataAnnotations;

namespace Listrr.Models
{
    public class CreateMovieListFileViewModel
    {
        [Required]
        [Display(Name = "List Name", Prompt = "List Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "List Items", Prompt = "List Items")]
        public string ItemList { get; set; }
    }
}