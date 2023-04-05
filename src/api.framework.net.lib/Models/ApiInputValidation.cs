using api.framework.net.Lib.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class ApiInputValidation
    {
        public ApiInputValidationType type { get; set; }
        public string error { get; set; } = string.Empty;
        public string errorCode { get; set; } = string.Empty;
        public string actiontobetaken { get; set; }
        public string regex { get; set; } = string.Empty;
        public string value { get; set; } = string.Empty;
        public string condition { get; set; } = string.Empty;
        public string fieldName { get; set; }
        public string name { get; set; }
        public string rangeStart { get; set; }
        public string rangeEnd { get; set; }
        public int maxLength { get; set; }
        public int minLength { get; set; }
        public List<BooleanExpression> expressions { get; set; }
        public string inputName { get; set; }

        /// <summary>
        /// Gets or Sets the flag for remove invalid data
        /// only applicable for delimited types
        /// </summary>
        public bool removeInvalid { get; set; } = false;
        public bool allowEmpty { get; set; } = false;
    }
}
