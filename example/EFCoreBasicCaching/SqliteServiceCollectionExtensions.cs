using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EFCoreSecondLevelCacheInterceptor;

namespace EFCoreBasicCaching
{
    public static class SqliteServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguredSqliteDbContext(this IServiceCollection services, string connectionString)
        {
            Console.WriteLine("AddCOnfiguredSqliteDbContext");
            services.AddDbContext<RestaurantContext>((serviceProvider, optionsBuilder) =>
                    optionsBuilder.UseSqlite(connectionString).AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
            return services;
        }
    }
}