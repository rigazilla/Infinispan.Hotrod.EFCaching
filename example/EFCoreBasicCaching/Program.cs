using Microsoft.Extensions.DependencyInjection;
using EFCoreSecondLevelCacheInterceptor;

namespace EFCoreBasicCaching
{
    internal class Program
    {
        static void Main(string[] args)
        {
                Console.WriteLine("Menu SQlLite");
                // Note: This sample requires the database to be created before running.
                // Console.WriteLine($"Database path: {db.DbPath}.");

                // Ensure the schema is created from scratch
                // db.Database.EnsureDeleted();
                // db.Database.EnsureCreated();
                // Create a menu
                Console.WriteLine("Inserting a new detached Menu item");
            EFServiceProvider.RunInContext(context =>
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Menus.Add(new Menu
                {
                    MenuId = 1,
                    MenuName = "Chilly Chicken",
                    MenuDescription = "Spicy chicken, fried with bell peppers in hot garlic sauce."
                });
                // context.SaveChanges();

                // Create a Order
                Console.WriteLine("Inserting a new Order with an entry");
                var items = new List<Menu>();
                items.Add(new Menu
                {
                    MenuId = 2,
                    MenuName = "Tomato Chicken",
                    MenuDescription = "Chicken, fried fresh tomatoes."
                });
                context.Orders.Add(new Order { OrderDate = DateTime.Now, TotalPrice = 20, MenuItems = items });
                context.SaveChanges();
                Console.WriteLine("Data inserted successfully.");
                for (int i=0; i<20; i++) {
                Console.WriteLine($"Order From Cache: {context.Orders.Cacheable().ToList().First().OrderId}");
                System.Threading.Thread.Sleep(1000);
                }
                Console.WriteLine($"Order From Cache: {context.Menus.Cacheable().ToList().First()}");
            });

        }
    }
}