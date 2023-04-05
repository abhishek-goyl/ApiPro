using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Data;
using System.Linq;

namespace api.framework.net.Lib
{
    public static class Extensions
    {
        public static JObject ToJObject(this object src)
        {
            JObject response = new JObject();
            try
            {
                string jSon = JsonConvert.SerializeObject(src);
                var type = src.GetType().FullName;
                
                if (type.StartsWith("System.Collections") && !type.Contains("Dictionary"))
                {
                    JObject temp = new JObject();
                    temp.Add("data", JArray.Parse(jSon));
                    return temp;
                }
                else
                {
                    response = JObject.Parse(jSon);
                }
            }
            catch { }
            return response;
        }

        public static JObject RemoveProperty(this object src, string property)
        {
            JObject res = new JObject();
            try
            {
                string jSon = JsonConvert.SerializeObject(src);
                res = JObject.Parse(jSon);
                res.Remove(property);
            }
            catch
            {
                return new JObject();
            }
            return res;
        }

        public static T ToObject<T>(this object src)
        {
            return JsonConvert.DeserializeObject<T>(src.ToJSONString());
        }

        public static JArray ToJArray(this string src)
        {
            try
            {
                return JArray.Parse(src);
            }
            catch
            {
                return new JArray();
            }
        }

        public static string ToJSONString(this object src)
        {
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(src, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                });
            }
            catch { }
            return json;
        }

        public static string ToCamelCase(this string str)
        {
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            words = words
                .Select(word => char.ToLower(word[0]) + word.Substring(1))
                .ToArray();
            return string.Join(string.Empty, words);
        }

        public static JObject ToJObject(this string src)
        {
            if(string.IsNullOrEmpty( src))
            {
                return new JObject();
            }
            return JObject.Parse(src);
        }
        public static T GetValue<T>(this DataRow src, string columnName)
        {
            T res = default(T);
            try
            {
                if (src[columnName] != null)
                {
                    res = (T)src[columnName];
                }
            }
            catch { }
            return res;
        }

        public static bool HasProperty(this JObject src, string propertyName)
        {
            bool flag = false;
            try
            {
                flag = src.Properties().Where(p => p.Name.Equals(propertyName)).Count() > 0;
            }
            catch { }
            return flag;
        }

        public static string TryParseString(this object src)
        {
            string res = string.Empty;
            try
            {
                res = src.ToString();
            }
            catch { }
            return res;
        }

        public static bool EvaluateBooleanExpression(this string expression)
        {
            bool res = true;
            try
            {
                res = Convert.ToBoolean(new DataTable().Compute(expression, null));
            }
            catch { }
            return res;
        }

    }
}
