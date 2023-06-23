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

                Console.WriteLine("\n\nRunning a new query on Orders: expect MISS");
                Console.WriteLine($"Order: {context.Orders.Cacheable().ToList().First().toJson()}");
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine("\n\nRunning same query again: expect HIT");
                Console.WriteLine($"Order: {context.Orders.Cacheable().ToList().First().toJson()}");
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine("\n\nAdding an Order to invalidate query");
                context.Orders.Add(new Order { OrderDate = DateTime.Now, TotalPrice = 20 });
                context.SaveChanges();
                Console.WriteLine("Running same query, but it has been invalidated: epect MISS");
                Console.WriteLine($"Order: {context.Orders.Cacheable().ToList().First().toJson()}");
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine("\n\nRunning a new query on Menus: expect MISS");
                Console.WriteLine($"Menu: {context.Menus.Cacheable().ToList().First().toJson()}");

                Console.WriteLine("\n\nAdding an Order, previous query on Menu shoudn't invalidate");
                context.Orders.Add(new Order { OrderDate = DateTime.Now, TotalPrice = 20 });
                Console.WriteLine("\n\nRunning same query again: expect HIT");
                Console.WriteLine($"Menu: {context.Menus.Cacheable().ToList().First().toJson()}");

                Console.WriteLine("\n\nRunning a new query on Order for a Menu: expect MISS");
                var menu = context.Orders.Where(o => o.OrderId==1).First().MenuItems.First();
                Console.WriteLine($"Menu: {menu.toJson()}");
                System.Threading.Thread.Sleep(1000);


                Console.WriteLine("\n\nRunning same query again: expect HIT");
                menu = context.Orders.Where(o => o.OrderId==1).First().MenuItems.First();
                Console.WriteLine($"Menu: {menu.toJson()}");


                Console.WriteLine("\n\nWaiting 15sec, cache expires in 10");
                System.Threading.Thread.Sleep(15000);
                Console.WriteLine("\n\nRunning same query, but cache is expired: expect MISS");
                menu = context.Orders.Where(o => o.OrderId==1).First().MenuItems.First();
                Console.WriteLine($"Menu: {menu.toJson()}");
            });
        }
    }   
}