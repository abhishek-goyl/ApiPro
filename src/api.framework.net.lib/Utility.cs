using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib
{
    public class Utility
    {
        public static string getDateForConstants(string constant, string pattern = null)
        {
            string srcVal = constant.ToLower();
            bool hasPattern = !string.IsNullOrEmpty(pattern);
            if (srcVal.Equals("today"))
                srcVal = DateTime.Now.ToString(hasPattern ? pattern : null);
            else if (srcVal.Contains("today") && srcVal.Contains("+"))
            {
                int ppart = 0;
                if (int.TryParse(srcVal.Substring("today+".Length), out ppart))
                {
                    srcVal = DateTime.Now.AddDays(ppart).ToString(hasPattern ? pattern : null);
                }
            }
            else if (srcVal.Contains("today") && srcVal.Contains("-"))
            {
                int mpart = 0;
                if (int.TryParse(srcVal.Substring("today+".Length), out mpart))
                {
                    mpart = 0 - mpart;
                    srcVal = DateTime.Now.AddDays(mpart).ToString(hasPattern ? pattern : null);
                }
            }
            return srcVal;
        }
    }
}
