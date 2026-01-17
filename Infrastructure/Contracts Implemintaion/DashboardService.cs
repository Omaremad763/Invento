using System.Text.Json;

using Application.Contracts;

using Domain.Entites;
using Domain.ReadOnlyObjects;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Repos
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public DashboardService(
            ApplicationDbContext context,
            IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IReadOnlyList<DashboardWidgetMetric>> GetMetricsAsync(
            CancellationToken cancellationToken)
        {
            const string cacheKey = "dashboard:metrics";

            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (cached is not null)
                return JsonSerializer.Deserialize<List<DashboardWidgetMetric>>(cached)!;

            var totalProducts = await _context.Products.CountAsync(cancellationToken);
            var totalStock = await _context.Products.SumAsync(p => p.StockQuantity, cancellationToken);

            var metrics = new List<DashboardWidgetMetric>
        {
            new() { Name = "Total Products", Value = totalProducts, Unit = "items" },
            new() { Name = "Total Stock", Value = totalStock, Unit = "units" }
        };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(metrics),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                },
                cancellationToken);

            return metrics;
        }

        public async Task<IReadOnlyList<TopProductResult>> GetTopProductsAsync(
            int limit,
            CancellationToken cancellationToken)
        {
            return await _context.StockTransactions
                .Where(t => t.StockTransactionType == StockTransactionTypeEnum.Sale)
                .GroupBy(t => new { t.ProductId, t.Product.Name })
                .Select(g => new TopProductResult
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalSoldQuantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSoldQuantity)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
    }

}
