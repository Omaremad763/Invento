using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;
using Application.Queries;

using MediatR;

namespace Application.Handlers
{
    public class GetDashboardMetricsHandler
      : IRequestHandler<GetDashboardMetricsQuery, IReadOnlyList<DashboardMetricDto>>
    {
        private readonly IDashboardService _repository;

        public GetDashboardMetricsHandler(IDashboardService repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<DashboardMetricDto>> Handle(
            GetDashboardMetricsQuery request,
            CancellationToken cancellationToken)
        {
            var metrics = await _repository.GetMetricsAsync(cancellationToken);

            return metrics
                .Select(m => new DashboardMetricDto(m.Name, m.Value, m.Unit))
                .ToList();
        }
    }
}
