using System.Collections.Generic;

namespace Listrr.Models
{
    public class PaginationViewModel<T>
    {
        public int Pages { get; set; }

        public int Page { get; set; }

        public IList<T> Items { get; set; }

    }
}