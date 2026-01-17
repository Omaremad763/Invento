using System.Linq.Expressions;

using Domain.Entites;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Dynamic Filter for all entities implementing ISoftDeletable
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(GetNonDeletedData(entityType.ClrType))
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
