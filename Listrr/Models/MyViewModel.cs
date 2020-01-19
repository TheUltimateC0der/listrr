using System.Collections.Generic;

using Listrr.Data;
using Listrr.Data.Trakt;

namespace Listrr.Models
{
    public class MyViewModel
    {

        public User User { get; set; }

        public IList<TraktList> TraktLists { get; set; }

    }
}
