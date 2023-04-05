using api.framework.net.Lib.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace api.framework.net.Lib
{
    public class GlobalApplicationVariables
    {
        HttpContext Context;
        public GlobalApplicationVariables()
        {
            this.Context = HttpContext.Current;
        }

        public GlobalApplicationVariables(HttpContext context)
        {
            this.Context = context;
        }

        public bool IsLocalDBConnectionEnabled
        {
            get { return Get<bool>("IsLocalDBConnectionEnabled"); }
            set { Set<bool>("IsLocalDBConnectionEnabled", value); }
        }

        public List<ApiError> PartialRequestError
        {
            get { return Get<List<ApiError>>("PartialRequestError"); }
            set { Set<List<ApiError>>("PartialRequestError", value); }
        }

        private T Get<T>(string key)
        {
            T res = default(T);
            try
            {
                if (this.Context.Items.Contains(key))
                {
                    res = (T)this.Context.Items[key];
                }
            }
            catch { }
            return res;
        }

        private void Set<T>(string key, T value)
        {
            try
            {
                if (this.Context.Items.Contains(key))
                {
                    if (typeof(T).IsArray || typeof(T).Namespace.Contains("Collection"))
                    { 
                        
                        JArray tmp = this.Context.Items[key].ToJSONString().ToJArray();
                        tmp.Merge(value.ToJSONString().ToJArray());
                        this.Context.Items[key] = tmp.ToObject<T>();
                    }
                    else
                    {
                        this.Context.Items[key] = value;
                    }
                }
                else
                {
                    this.Context.Items.Add(key, value);
                }
            }
            catch { }
        }
    }
}
