using Antlr.Runtime;
using ClickAndCollect.Auth;
using ClickAndCollect.Data;
using ClickAndCollect.IOC;
using ClickAndCollect.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity;
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
            container.RegisterType<ITokenManager, JwtTokenManager>(new SingletonLifetimeManager());
            container.RegisterType<ILogger,Log4netLogger>(new SingletonLifetimeManager());
            container.RegisterType<ITokenStore,SQLTokenStore>();
            config.DependencyResolver = new UnityResolver(container);
        }
    }
}
