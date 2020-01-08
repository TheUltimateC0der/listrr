using System.ComponentModel.DataAnnotations;

namespace Listrr.API.Trakt.Models.Filters
{
    public class RatingsCommonFilter
    {

        [Range(0,100)]
        [Display(Name = "Min rating", Prompt = "0")]
        public int From { get; set; } = 0;

        [Range(0, 100)]
        [Display(Name = "Max rating", Prompt = "100")]
        public int To { get; set; } = 0;

        [Range(0, int.MaxValue)]
        [Display(Name = "Min Votes", Prompt = "10")]
        public int Votes { get; set; } = 0;

    }
}