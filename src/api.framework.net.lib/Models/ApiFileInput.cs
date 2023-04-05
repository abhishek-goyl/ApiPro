using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
   public class ApiFileInput
    {
        public string type { get; set; } = ".xlsx";
        public List<WorkSheet> worksheets { get; set; } = new List<WorkSheet>();
    }
}
