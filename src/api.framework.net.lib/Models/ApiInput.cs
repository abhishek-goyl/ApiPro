using api.framework.net.Lib.Models.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace api.framework.net.Lib.Models
{
    public class ApiInput
    {
        public ApiInput()
        { }

        public ApiInput(ApiInput input)
        {
            this.id = input.id;
            this.authId = input.authId;
            this.defaultValue = input.defaultValue;
            this.description = input.description;
            this.@enum = input.@enum;
            this.name = input.name;
            this.pattern = input.pattern;
            this.separator = input.separator;
            this.source = input.source;
            this.type = input.type;
            this.validations = input.validations;
            this.conditionalDefaults = input.conditionalDefaults;
            this.ValidationErrors = new List<ApiError>();
            this.file = input.file;
            this.ignoreValidation = input.ignoreValidation;
        }


        /// <summary>
        /// Gets or Sets the unique Id of the input which is used to map with operation inputs
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or Sets the unique Id for auth claim type inputs
        /// </summary>
        public int authId { get; set; }

        /// <summary>
        /// Gets or Sets the name of the Api Input
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the value of the input, which is set at the run time
        /// </summary>
        public object value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the integer value of enum
        /// </summary>
        public int? enumInt { get; set; }

        /// <summary>
        /// Gets or Sets the string value of enum
        /// </summary>
        public string enumString { get; set; }

        /// <summary>
        /// Gets or Sets the example value to be displayed on swagger documentation
        /// </summary>
        public object example { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the enum values for the input
        /// </summary>
        public Dictionary<string, int> @enum { get; set; }

        /// <summary>
        /// Gets or Sets the .net type name of the input
        /// </summary>
        public ApiInputType type { get; set; } = ApiInputType.@string;

        /// <summary>
        /// Gets or Sets the data format
        /// </summary>
        public string pattern { get; set; }

        /// <summary>
        /// Gets or Sets the validations for the input
        /// </summary>
        public List<ApiInputValidation> validations { get; set; } = new List<ApiInputValidation>();

        /// <summary>
        /// Gets or Sets the validation Errors
        /// </summary>
        public List<ApiError> ValidationErrors { get; set; } = new List<ApiError>();

        /// <summary>
        /// Gets or Sets the default value for the input
        /// </summary>
        public object defaultValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the description of the input
        /// </summary>
        public string description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the source of the input. Possible values are path/query/header/body
        /// </summary>
        public ApiInputSources source { get; set; }

        /// <summary>
        /// Gets or Sets the separator only applicable in case of delimited input
        /// </summary>
        public string separator { get; set; }

        /// <summary>
        /// Gets or Sets the conditional defaults.
        /// it helps to define the conditional default value for the input
        /// </summary>
        public List<ConditionalDefault> conditionalDefaults { get; set; }

        /// <summary>
        /// Get or Sets the file properties(type and details) applicable in case of file type.
        /// </summary>
        public ApiFileInput file { get; set; }
        /// <summary>
        /// Get or Sets the ignoreValidation for the input 
        /// </summary>
        public bool ignoreValidation { get; set; }

        /// <summary>
        /// Gets or Sets the posted data properties (for posting multiple recordsets)
        /// </summary>
        public PostedDataProperties DataProperties { get; set; }

        /// <summary>
        /// Gets the combined validation error for each column
        /// </summary>
        public string validationErrorMessage
        {
            get
            {
                string _error = string.Empty;
                var _temp = this.ValidationErrors.Select(x => x.errorMessage).ToList();
                _temp.RemoveAll(x => string.IsNullOrEmpty(x));
                _error = string.Join(", ", _temp);
                return _error;
            }
        }

        /// <summary>
        ///  Gets the combined Action to be taken for each column (for file upload)
        /// </summary>
        public string actionToBetaken
        {
            get
            {
                string _actionToBetaken = string.Empty;
                var _temp = this.ValidationErrors.Select(x => x.actionToBeTaken).ToList();
                _temp.RemoveAll(x => string.IsNullOrEmpty(x));
                _actionToBetaken = string.Join(", ", _temp);
                return _actionToBetaken;
            }
        }

        /// <summary>
        /// Gets or Sets the input of individual data of the posted file.
        /// </summary>
        public List<ApiInput> FileInputs { get; set; }
    }
}
