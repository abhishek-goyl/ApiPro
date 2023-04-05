using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
  public  class WorkSheet
    {
        public string id { get; set; }
        public string name { get; set; }
        public string mappingName { get; set; }

        /// <summary>
        /// Gets or Sets the data orientation in uploaded file
        /// </summary>
        public string orientation { get; set; }

        /// <summary>
        /// Gets or Sets format for the file data, i.e. xml or JSON
        /// </summary>
        public string format { get; set; }

        /// <summary>
        /// Gets or Sets the flag for adding a new column in data for identification on each record in file upload.
        /// </summary>
        public bool includeindex { get; set; }

        /// <summary>
        /// Gets or Sets the name for index column (Used for file upload having multiple records to identify each record saperatly)
        /// </summary>
        public string indexname { get; set; }

        /// <summary>
        /// Gets or Sets the columns for file having multiple recordsets.
        /// </summary>
        public List<DataColumn> columns { get; set; }

        /// <summary>
        /// Gets or Sets the flag for ignoring validation errors for execution of Opertaion and send directly in response (merged with operation output)
        /// </summary>
        public bool ignoreValidation { get; set; }

        /// <summary>
        /// Gets or Sets the validations for the worksheet
        /// </summary>
        public List<ApiInputValidation> validations { get; set; } = new List<ApiInputValidation>();
    }
}
