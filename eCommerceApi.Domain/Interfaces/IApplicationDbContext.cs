using System.Threading;
using System.Threading.Tasks;
using eCommerceApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApi.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<ProductCategory> productCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Role> Roles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
