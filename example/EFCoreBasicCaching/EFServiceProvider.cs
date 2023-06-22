using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using EFCoreSecondLevelCacheInterceptor;

namespace EFCoreBasicCaching
{
    public static class EFServiceProvider
    {
        private static readonly Lazy<IServiceProvider> _serviceProviderBuilder =
                new Lazy<IServiceProvider>(getServiceProvider, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// A lazy loaded thread-safe singleton
        /// </summary>
        public static IServiceProvider Instance { get; } = _serviceProviderBuilder.Value;

        public static T GetRequiredService<T>()
        {
            return Instance.GetRequiredService<T>();
        }

        public static void RunInContext(Action<RestaurantContext> action)
        {
            using var serviceScope = GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<RestaurantContext>();
            action(context);
        }

        public static async Task RunInContextAsync(Func<RestaurantContext, Task> action)
        {
            using var serviceScope = GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<RestaurantContext>();
            await action(context);
        }

        private static IServiceProvider getServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddOptions();

            services.AddLogging(cfg => cfg.AddConsole().AddDebug().AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning));
            services.AddEFSecondLevelCache(options => 
            {
                options.UseCustomCacheProvider<EFInfinispanCacheServiceProvider>().UseCacheKeyPrefix("EF_").CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromSeconds(10));
            });

            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"Using `{basePath}` as the ContentRootPath");
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(basePath)
                                .Build();
            services.AddSingleton(_ => configuration);
            services.AddConfiguredSqliteDbContext(getConnectionStringLite(basePath, configuration));

            return services.BuildServiceProvider();
        }


        private static string getConnectionStringLite(string basePath, IConfigurationRoot configuration)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var cf = "Data Source="+System.IO.Path.Join("/home/rigazilla/", "Restaurant.db");
            Console.WriteLine("COnnection String: "+cf);
            return cf;
        }

        private static string getConnectionString(string basePath, IConfigurationRoot configuration)
        {
            var testsFolder = basePath.Split(new[] { "\\Tests\\" }, StringSplitOptions.RemoveEmptyEntries)[0];
            var contentRootPath = Path.Combine(testsFolder, "Tests", "EFCoreSecondLevelCacheInterceptor.AspNetCoreSample");
            var connectionString = configuration["ConnectionStrings:ApplicationDbContextConnection"];
            if (connectionString.Contains("%CONTENTROOTPATH%"))
            {
                connectionString = connectionString.Replace("%CONTENTROOTPATH%", contentRootPath);
            }
            Console.WriteLine($"Using {connectionString}");
            return connectionString;
        }
    }
}