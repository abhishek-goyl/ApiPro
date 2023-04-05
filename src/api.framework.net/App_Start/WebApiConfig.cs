using api.framework.net.Filters;
using api.framework.net.Handlers;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models.Enums;
using api.framework.net.Lib.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Http;

namespace api.framework.net
{
    public static class WebApiConfig
    {
        public static IConfiguration appConfig = new ConfigurationProvider();

        public static void Register(HttpConfiguration config)
        {
            // Web API all dependencies and services
            UnityConfig.RegisterComponents();

            // Web API register the Exception Filter
            config.Filters.Add(new ApiExceptionFilter());

            // Web API register the Message Handlers
            config.MessageHandlers.Add(new CustomHeaders());
            config.MessageHandlers.Add(new ApiLoggingHandler());
            
            // Web API custom schema and generating custom swagger json
            IApiSchemaProvider schemaProvider = new ApiSchemaProvider(appConfig);
            ApiSchemaProvider.LoadSchema(appConfig);
            schemaProvider.GenerateSwaggerDocument();
            // Web API configure routes

            var schema = schemaProvider.GetSchema();

            config.MapHttpAttributeRoutes();
            
            dynamic routeDefaults2 = new ExpandoObject();
            IDictionary<string, object> myUnderlyingObject2 = routeDefaults2;
            myUnderlyingObject2.Add("controller", "Common");
            myUnderlyingObject2.Add("action", "Index");
            myUnderlyingObject2.Add("version", RouteParameter.Optional);
            myUnderlyingObject2.Add("tag", RouteParameter.Optional);
            myUnderlyingObject2.Add("name", RouteParameter.Optional);
            var optionalRoutParameters2 = new List<string>();
            try
            {
                schema.endpoints.ForEach(e => optionalRoutParameters2.AddRange(e.inputs.FindAll(input =>
                        input.source == ApiInputSources.path
                        && input.validations.FindAll(v => v.type == ApiInputValidationType.mandatory).Count == 0).Select(n => n.name)));
                foreach (var opp in optionalRoutParameters2)
                {
                    myUnderlyingObject2.Add(opp, RouteParameter.Optional);
                }
            }
            catch { }
            config.Routes.MapHttpRoute(
                        name: "sample",
                        routeTemplate: "api/{version}/{tag}/{name}",
                        defaults: myUnderlyingObject2
                    );

            // Web API mock data
            config.Routes.MapHttpRoute(
                        name: "mockRoute",
                        routeTemplate: "mock",
                        defaults: new { controller = "Mock" }
                    );

            // Web API configure the swagger doc route
            config.Routes.MapHttpRoute(
                        name: "swagger",
                        routeTemplate: "swagger/doc/v1",
                        defaults: new { controller = "Swagger" }
                    );



            //// Web API health route
            config.Routes.MapHttpRoute(
                        name: "health",
                        routeTemplate: "",
                        defaults: new { controller = "Health" }
                    );

            // Web API configure the 404 route
            //config.routes.maphttproute(
            //            name: "notfound",
            //            routetemplate: "{basepath}/{version}/{tag}/{name}/{id}/{par1}/{par2}",
            //            defaults: new {
            //                controller = "notfound",
            //                basepath = routeparameter.optional,
            //                version = routeparameter.optional,
            //                tag = routeparameter.optional,
            //                name = routeparameter.optional,
            //                id = routeparameter.optional,
            //                par1 = routeparameter.optional,
            //                par2 = routeparameter.optional
            //            }
            //        );

            // Web API Json Formatter
            config.Formatters.Add(new BrowserJsonFormatter());
        }

    }
}
