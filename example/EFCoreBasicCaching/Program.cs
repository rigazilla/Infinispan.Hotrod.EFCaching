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
            
            // Ensure the schema is created from scratch
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            // Create a menu
            Console.WriteLine("Inserting a new detached Menu item");
            db.Add(new Menu {MenuId= 1, MenuName = "Chilly Chicken", 
                MenuDescription = "Spicy chicken, fried with bell peppers in hot garlic sauce." });
            db.SaveChanges();

            // Create a Order
            Console.WriteLine("Inserting a new Order with an entry");
            var items = new List<Menu>();
            items.Add (new Menu {MenuId= 2, MenuName = "Tomato Chicken", 
                MenuDescription = "Chicken, fried fresh tomatoes." });
            db.Add(new Order { OrderDate = DateTime.Now,  TotalPrice = 20, MenuItems =  items});
            db.SaveChanges();
            Console.WriteLine("Data inserted successfully.");

        }


    }
}