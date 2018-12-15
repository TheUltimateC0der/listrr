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
                From = Convert.ToUInt32(rating.Split("-")[0]);
                To = Convert.ToUInt32(rating.Split("-")[1]);
            }
            else
            {
                From = 0;
                To = Convert.ToUInt32(rating);
            }
        }

        [Display(Name = "Min runtime", Prompt = "30")]
        public uint From { get; set; } = 0;

        [Display(Name = "Max runtime", Prompt = "200")]
        public uint To { get; set; } = 0;

    }
}