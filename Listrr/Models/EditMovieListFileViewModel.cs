using System.ComponentModel.DataAnnotations;

namespace Listrr.Models
{
    public class EditMovieListFileViewModel : CreateMovieListFileViewModel
    {
        [Required]
        public uint Id { get; set; }

        [Display(Name = "Report", Prompt = "Report")]
        public string Report { get; set; }
    }
}