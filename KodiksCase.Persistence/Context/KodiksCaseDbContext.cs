using KodiksCase.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Persistence.Context
{
    public class KodiksCaseDbContext : DbContext
    {
        public KodiksCaseDbContext(DbContextOptions<KodiksCaseDbContext> options) : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>().HasData(
                new User { Id = Guid.Parse("046a3a78-5ebb-489f-b7b8-08a6839fcab2"), FullName = "Kodiks Test", Email = "test@kodiks.com", Password = "AQAAAAIAAYagAAAAEK8w0qXxszvuE+7qBwzd7niDYPfxHkJqDbIdQy7tKoXHcPLyYsQNevYFDCgt6g7ZHA==" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = Guid.Parse("d0696b26-44b8-420c-855b-eac593582258"), Name = "Product 1", Price = 1500.00m },
                new Product { Id = Guid.Parse("49a382af-58f4-422b-a726-0408332d12bd"), Name = "Product 2", Price = 500.00m }
            );
        }
    }
}
