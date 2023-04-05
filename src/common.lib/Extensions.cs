using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nippon.CommonLib
{
    public static class Extensions
    {
        public static T getValue<T> (this string[] src, int index)
        {
            T res = default(T);
            try
            {
                if (src.Length > index)
                {
                    res = (T)Convert.ChangeType(src[index], typeof(T));
                }
            }
            catch { }
            return res;
        }
    }
}
