using Antlr.Runtime;
using ClickAndCollect.Auth;
using ClickAndCollect.Data;
using ClickAndCollect.IOC;
using ClickAndCollect.Logs;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace ClickAndCollect
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Log configuration
            log4net.Config.XmlConfigurator.Configure();

            //Dependecy Injection configuration

            var container = new UnityContainer();
            // Logger log4net
            container.RegisterType<ILogger, Log4netLogger>(new SingletonLifetimeManager());
        
            // Cache with SQL Server Store configuration
            container.RegisterFactory<ITokenStore>((unityContainer) => {
                var connectionString = ConfigurationManager.ConnectionStrings["token"];
                if (connectionString == null) throw new ApplicationException("Not token connectionString defined");
                return new CacheTokenStore(new SQLTokenStore(connectionString.ConnectionString, unityContainer.Resolve<ILogger>()));
            }, new SingletonLifetimeManager());

            // Token Manager
            container.RegisterType<ITokenManager, JwtTokenManager>(new SingletonLifetimeManager());


            config.DependencyResolver = new UnityResolver(container);
        }
    }
}
