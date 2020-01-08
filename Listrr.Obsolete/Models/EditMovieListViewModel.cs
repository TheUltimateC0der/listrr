using System.ComponentModel.DataAnnotations;

namespace Listrr.Models
{
    public class EditMovieListViewModel : CreateMovieListViewModel
    {
        [Required]
        public uint Id { get; set; }

    }
}