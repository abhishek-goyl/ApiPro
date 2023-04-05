using api.framework.net.Lib.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class DbInput
    {
        /// <summary>
        /// Gets or Sets the source, only applicable to map from previous operations
        /// </summary>
        public DbInputSources source { get; set; }

        /// <summary>
        /// Gets or Sets the map id to get the value from endpoint inputs with matching id
        /// </summary>
        public string mapId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the name of the input parameter for sp or query 
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the flag for optional
        /// </summary>
        public bool optional { get; set; }

        /// <summary>
        /// Gets or Sets the DB type
        /// </summary>
        public DbTypes type { get; set; }

        /// <summary>
        /// Gets or sets the flag which decide the number time this operation will be executed
        /// </summary>
        public bool allowMultiple { get; set; }

        /// <summary>
        /// Gets or Sets the default value for the input
        /// </summary>
        public string defaultValue { get; set; }

        /// <summary>
        /// Gets or Sets the string value for the input
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Gets or Sets the SQL parameter direction, by default its input.
        /// value of output will be part of API response at root, with the name of parameter (only if specified in Output)
        /// </summary>
        public DbParameterDirection direction { get; set; } = DbParameterDirection.Input;
    }
}
