using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.ReadOnlyObjects;

namespace Application.Contracts
{
    public interface IDashboardService
    {
        Task<IReadOnlyList<DashboardWidgetMetric>> GetMetricsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<TopProductResult>> GetTopProductsAsync(
            int limit,
            CancellationToken cancellationToken);
    }
}
