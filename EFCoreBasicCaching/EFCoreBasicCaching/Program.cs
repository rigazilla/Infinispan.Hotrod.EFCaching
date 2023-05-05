using System.Reflection.Metadata;

namespace EFCoreBasicCaching
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Menu SQlLite");
            using var db= new RestaurantContext();
            // Note: This sample requires the database to be created before running.
            Console.WriteLine($"Database path: {db.DbPath}.");
            
            // Ensure the schema is created
            db.Database.EnsureCreated();
            // Create a menu
            Console.WriteLine("Inserting a new Menu");
            db.Add(new Menu {MenuId= 1, MenuName = "Chilly Chicken", 
                MenuDescription = "Spicy chicken, fried with bell peppers in hot garlic sauce." });
            db.SaveChanges();

            // Create a Order
            Console.WriteLine("Inserting a new Order");
            db.Add(new Order { OrderDate = DateTime.Now,  TotalPrice = 20 });
            db.SaveChanges();
            Console.WriteLine("Data inserted successfully.");

        }


    }
}