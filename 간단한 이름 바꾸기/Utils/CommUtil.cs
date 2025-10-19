using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 간단한_이름_바꾸기.Utils
{
    public static class CommUtil
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return true;
            return false;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) return true;
            return false;
        }
    }
}
