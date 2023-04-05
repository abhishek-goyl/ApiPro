using api.framework.net.ExceptionLib;
using api.framework.net.Lib.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Models
{
    public class BooleanExpression 
    {
        public string inputName { get; set; }
        public string fieldName { get; set; }
        public string value { get; set; }
        public string condition { get; set; }

        public bool Evaluate(List<ApiInput> inputs, int rowIndex = -1)
        {
            bool res = true;
            var field = rowIndex == -1? GetInput(this.inputName, inputs): GetInput(this.inputName, inputs, rowIndex);
            var firstArgument = field.value;
            var secondArgument = !string.IsNullOrEmpty(this.fieldName) 
                ? GetInput(this.fieldName, inputs).value
                : GetValue(this.value, field.type, field.pattern);
            switch (field.type)
            {
                case ApiInputType.date:
                    DateTime date1 = DateTime.Parse(firstArgument.TryParseString()).Date;
                    DateTime date2 = string.IsNullOrEmpty(field.pattern) 
                        ? DateTime.Parse(secondArgument.TryParseString()).Date
                        : DateTime.ParseExact(secondArgument.TryParseString(), field.pattern, CultureInfo.InvariantCulture).Date;
                    res = Compare<DateTime>(date1, date2);
                    break;
                case ApiInputType.datetime:
                    DateTime dateTime1 = DateTime.Parse(firstArgument.TryParseString());
                    DateTime dateTime2 = DateTime.Parse(secondArgument.TryParseString());
                    res = Compare<DateTime>(dateTime1, dateTime2);
                    break;

                case ApiInputType.@int:
                case ApiInputType.@long:
                case ApiInputType.@double:
                    double d1 = double.Parse(firstArgument.TryParseString());
                    double d2 = double.Parse(secondArgument.TryParseString());
                    res = Compare<double>(d1, d2);
                    break;
                case ApiInputType.@string:
                    string s1 = firstArgument.TryParseString().ToLower();
                    string s2 = secondArgument.TryParseString().ToLower();
                    res = Compare<string>(s1, s2);
                    break;
                default:
                    break;
            }
            return res;
        }

        private string GetValue(string value, ApiInputType type, string pattern)
        {
            string res = value;
            switch (type)
            {
                case ApiInputType.datetime:
                case ApiInputType.date:
                    res = Utility.getDateForConstants(value, pattern);
                    break;
                default:
                    break;
            }
            return res;
        }

        private ApiInput GetInput(string name, List<ApiInput> inputs , int rowIndex = -1)
        {
            var field = rowIndex == -1 ? 
                        inputs.Find(i => i.name.Equals(name, StringComparison.InvariantCultureIgnoreCase)):
                         inputs.Find(i => i.name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && i.DataProperties.index == rowIndex);
            
            if (field == null)
            {
                throw new AppException(string.Format("There is no input with name {0}. Please correct the configuration", name), "InvalidConfiguration");
            }
            return field;
        }

        private bool Compare<T>(T arg1, T arg2) where T : IComparable
        {
            bool isValid = true;
            switch (condition)
            {
                case ">":
                    isValid = arg2.CompareTo(arg1) < 0;
                    break;
                case ">=":
                    isValid = arg2.CompareTo(arg1) < 1;
                    break;
                case "=":
                case "==":
                    isValid = arg2.CompareTo(arg1) == 0;
                    break;
                case "!=":
                    isValid = arg2.CompareTo(arg1) != 0;
                    break;
                case "<":
                    isValid = arg2.CompareTo(arg1) > 0;
                    break;
                case "<=":
                    isValid = arg2.CompareTo(arg1) > -1;
                    break;
            }

            return isValid;
        }
    }
}
