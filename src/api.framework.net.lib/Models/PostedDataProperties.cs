using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class PostedDataProperties
    {
        /// <summary>
        /// Gets or Sets the reference of index from uploaded file (applicable only for file upload)
        /// </summary>
        public int index { get; set; }

        /// <summary>
        /// Gets or Sets the flag for having index.
        /// </summary>
        public bool requireIndex { get; set; }

        /// <summary>
        /// Gets or Sets the table name to which input belong
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Gets or Sets the reference of indexname of worksheet to add row number tag.
        /// </summary>
        public string indexname { get; set; }
    }
}
