using BookShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BookShop.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Comment> Comments { get; }
        DbSet<Order> Orders { get; }
        DbSet<Book> Books { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<ApplicationUser> ApplicationUsers { get; }
        Task<int> SaveChangesAsync();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
    }
}
