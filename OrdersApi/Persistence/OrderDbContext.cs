using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrdersApi.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistence
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> dbContextOptions):base(dbContextOptions)
        {

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var convertor = new EnumToStringConverter<Status>();
            modelBuilder.Entity<Order>().Property(p => p.Status).HasConversion(convertor);

            //configuring relationship
            modelBuilder.Entity<Order>().HasKey(k => k.OrderId);
            modelBuilder.Entity<OrderDetail>().HasKey(k => k.OrderDetailId);
            modelBuilder.Entity<Order>().HasMany(o => o.OrderDetails).WithOne(o => o.Order).HasForeignKey(f => f.OrderId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }

        public void MigrateDb()
        {
            Policy.Handle<Exception>()
                .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
                .Execute(() => Database.Migrate());
        }
    }
}
