namespace Microsoft.Extensions.DependencyInjection
{
    using Maincotech.Blazor;
    using Maincotech.Blazor.Routing;
    using Maincotech.Erp;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    public static class ErpBlazorExtensions
    {
        public static IServiceCollection AddErpBlazor([NotNull] this IServiceCollection services, [NotNull] Action<IServiceProvider, ErpOptionsBuilder> optionAction)
        {
            var builder = new ErpOptionsBuilder();

            //ViewModels
            services.AddScoped<Maincotech.Erp.Pages.Product.IndexViewModel>();

            //We need add routes and layouts immediately. So we create service provider here.
            using var serviceProvider = services.BuildServiceProvider();
            optionAction.Invoke(serviceProvider, builder);
            services.AddSingleton(builder.Options);

            RegisterLayout(serviceProvider, builder.Options);
            RegisterRoutes(serviceProvider, builder.Options);
            return services;
        }

        private static void RegisterRoutes(IServiceProvider serviceProvider, ErpOptions options)
        {
            var appAssembly = Assembly.GetExecutingAssembly();
            var routeManager = serviceProvider.GetRequiredService<RouteManager>();
            routeManager.RegisterRoutesInAssembly(appAssembly, options.AreaName);
        }

        private static void RegisterLayout(IServiceProvider serviceProvider, ErpOptions options)
        {
            if (options.Layout != null)
            {
                var layoutProvider = serviceProvider.GetRequiredService<ILayoutProvider>();

                var appAssembly = Assembly.GetExecutingAssembly();

                var pageComponentTypes = appAssembly
                    .ExportedTypes
                    .Where(t => t.Namespace != null && (t.IsSubclassOf(typeof(ComponentBase))
                                                        && t.Namespace.Contains(".Pages")));

                foreach (var pageType in pageComponentTypes)
                {
                    if (pageType.FullName == null)
                        continue;
                    layoutProvider.Register(pageType.FullName, options.Layout);
                }
            }
        }
    }
}