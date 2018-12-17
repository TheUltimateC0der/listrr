using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class YearsCommonFilter
    {

        public YearsCommonFilter()
        {
            
        }

        public YearsCommonFilter(string rating)
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

        [Display(Name = "Min year", Prompt = "1990")]
        public int? From { get; set; }

        [Display(Name = "Max year", Prompt = "2018")]
        public int? To { get; set; }

    }
}