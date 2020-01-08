using System.ComponentModel.DataAnnotations;

namespace Listrr.Models
{
    public class EditShowListViewModel : CreateShowListViewModel
    {
        [Required]
        public uint Id { get; set; }

    }
}