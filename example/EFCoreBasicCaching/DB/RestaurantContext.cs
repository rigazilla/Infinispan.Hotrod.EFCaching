using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreBasicCaching
{
    internal class RestaurantContext : DbContext
    {
        public DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        public string DbPath { get; set; }
        public RestaurantContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "Restaurant.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {

                                //Model builder for menu
                modelBuilder.Entity<Menu>(entity =>
                {
                    entity.HasKey(e => e.MenuId);
                    entity.Property(e => e.MenuId);
                    entity.Property(e => e.MenuName).HasMaxLength(150).IsRequired();
                    entity.Property(e => e.MenuDescription).HasMaxLength(250);
                    entity.Property(e => e.Price).HasDefaultValue(1.75);
                    entity.ToTable(nameof(Menu));
                });
                // Model builder for order
                modelBuilder.Entity<Order>
                    (entity =>
                    {
                        entity.HasKey(e => e.OrderId);

                        entity.Property(e => e.OrderId);
                        // entity.Property(e => e.MenuId);
                        entity.Property(e => e.OrderDate);
                        entity.ToTable(nameof(Order));
                    });
                // modelBuilder.Entity<Order>().HasMany(e=>e.MenuItems).WithOne().HasForeignKey(e=>e.MenuId).HasPrincipalKey(e=>e.OrderId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
