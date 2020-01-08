using System.Collections.Generic;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Comparer
{
    public class TraktShowComparer : IEqualityComparer<ITraktShow>
    {
        public bool Equals(ITraktShow x, ITraktShow y)
        {
            return x.Ids.Slug == y.Ids.Slug;
        }

        public int GetHashCode(ITraktShow obj)
        {
            return obj.GetHashCode();
        }
    }
}
