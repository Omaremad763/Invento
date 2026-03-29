using System.Linq.Expressions;

using Domain.Entites;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Dynamic Filter for all entities implementing ISoftDeletable
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).HasQueryFilter(GetNonDeletedData(entityType.ClrType))
                    .HasIndex(nameof(BaseEntity.IsDeleted))
                    .HasFilter($"\"{nameof(BaseEntity.IsDeleted)}\" = false");
                }
            }

        }
        //expression tree to filter IsDeleted = false
        private static LambdaExpression GetNonDeletedData(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var body = Expression.Equal(Expression.Property(parameter, "IsDeleted"), Expression.Constant(false));
            return Expression.Lambda(body, parameter);
        }
    }
}
