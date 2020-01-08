using System.Collections.Generic;
using TraktNet.Objects.Get.Movies;

namespace Listrr.Comparer
{
    public class TraktMovieComparer : IEqualityComparer<ITraktMovie>
    {
        public bool Equals(ITraktMovie x, ITraktMovie y)
        {
            return x.Ids.Slug == y.Ids.Slug;
        }

        public int GetHashCode(ITraktMovie obj)
        {
            return obj.GetHashCode();
        }
    }
}