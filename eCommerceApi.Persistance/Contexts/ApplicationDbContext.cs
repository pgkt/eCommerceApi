using eCommerceApi.Domain.Entities;
using eCommerceApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApi.Persistance.Contexts
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<ProductCategory> productCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
