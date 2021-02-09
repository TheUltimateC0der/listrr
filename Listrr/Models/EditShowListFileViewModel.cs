using System.ComponentModel.DataAnnotations;

namespace Listrr.Models
{
    public class EditShowListFileViewModel : CreateShowListFileViewModel
    {
        [Required]
        public uint Id { get; set; }

        [Display(Name = "Report", Prompt = "Report")]
        public string Report { get; set; }
    }
}