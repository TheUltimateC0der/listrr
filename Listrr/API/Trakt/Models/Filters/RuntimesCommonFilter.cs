using System.ComponentModel.DataAnnotations;

namespace Listrr.API.Trakt.Models.Filters
{
    public class RuntimesCommonFilter
    {

        [Display(Name = "Min runtime", Prompt = "30")]
        public int From { get; set; }

        [Display(Name = "Max runtime", Prompt = "200")]
        public int To { get; set; }

    }
}