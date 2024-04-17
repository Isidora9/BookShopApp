using BookShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Comment> Comments { get; }
    }
}
