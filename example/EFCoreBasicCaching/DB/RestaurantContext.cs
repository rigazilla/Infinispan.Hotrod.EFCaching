using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using EFCoreSecondLevelCacheInterceptor;

namespace EFCoreBasicCaching
{
    public class RestaurantContext : DbContext
    {
        public DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options)
        {
            // See hint in ApplicationDbContext class of EFCoreSecondLevelCacheInterceptor Tests
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Always;
        }
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
                        entity.Property(e => e.OrderDate);
                        entity.ToTable(nameof(Order));
                    });
            }
            catch (Exception )
            {
                throw;
            }
        }
    }
}
