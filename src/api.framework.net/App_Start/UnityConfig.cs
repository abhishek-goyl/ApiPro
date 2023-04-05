using api.framework.net.business.BusinessAuthorzation;
using api.framework.net.business.Contract;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Providers;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace api.framework.net
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<IRequestProvider, RequestProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<ISqlDataProvider, SqlDataProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<IBusinessAuthorize, ApiBusinessAuthorization>(new HierarchicalLifetimeManager());
            container.RegisterType<IBusinessProvider, BusinessProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<IApiSchemaProvider, ApiSchemaProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<IConfiguration, ConfigurationProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<IMockSqlProvider, MockSqlProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<ICacheProvider, MemoryCacheProvider>(new HierarchicalLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}