using Antlr.Runtime;
using ClickAndCollect.Auth;
using ClickAndCollect.Data;
using ClickAndCollect.IOC;
using ClickAndCollect.Logs;
using ClickAndCollect.Proxies;
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

            // Token Store
            AddTokenStoreFactory(container);

            // Token Manager
            AddTokenManagerFactory(container);
            
            // Service Manager
            container.RegisterType<IServiceProxy, TestServiceProxy>(new SingletonLifetimeManager());

            config.DependencyResolver = new UnityResolver(container);
        }

        public static void AddTokenStoreFactory(UnityContainer container)
        {
            // Cache with SQL Server Store configuration
            container.RegisterFactory<ITokenStore>((unityContainer) =>
            {
                var connectionString = ConfigurationManager.ConnectionStrings["token"];
                if (connectionString == null) throw new ApplicationException("Not token connectionString defined");
                return new CacheTokenStore(new SQLTokenStore(connectionString.ConnectionString, unityContainer.Resolve<ILogger>()));
            }, new SingletonLifetimeManager());
        }

        public static void AddTokenManagerFactory(UnityContainer container)
        {
            container.RegisterFactory<ITokenManager>((unityContainer) =>
            {
                var logger = unityContainer.Resolve<ILogger>();
                int expireMinutes = 0;
                int.TryParse(ConfigurationManager.AppSettings["tokenExpireMinutes"], out expireMinutes);

                if (expireMinutes == 0)
                {

                    expireMinutes = 20;
                    logger.Info($"Set default token expire to {expireMinutes} minutes");
                }
                else
                {
                    logger.Info($"Set token expire to {expireMinutes} minutes");
                }



                // Da verificare dove mettere il secret: scritto a codice o su web.config (criptato?)
                const string secret = "853BDA13BAC62B7224AF452F5658FBE01D0FDEBD0B1BD7638F289B37D3C06E44";
                return new JwtTokenManager(logger, secret, expireMinutes);
            }, new SingletonLifetimeManager());

        }
    }
}
