using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class RuntimesCommonFilter
    {

        public RuntimesCommonFilter()
        {
            
        }

        public RuntimesCommonFilter(string rating)
        {
            if (rating.Contains("-"))
            {
                From = Convert.ToInt32(rating.Split("-")[0]);
                To = Convert.ToInt32(rating.Split("-")[1]);
            }
            else
            {
                From = 0;
                To = Convert.ToInt32(rating);
            }
        }

        [Display(Name = "Min runtime", Prompt = "30")]
        public int From { get; set; }

        [Display(Name = "Max runtime", Prompt = "200")]
        public int To { get; set; }

    }
}