using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class ConditionalDefault
    {
        public List<BooleanExpression> expressions { get; set; }
        public string condition { get; set; }
        public string value { get; set; }
        public string claimValue { get; set; }
        public string fieldValue { get; set; }
    }
}
