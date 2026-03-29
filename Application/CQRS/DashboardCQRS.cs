using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using AutoMapper;

using MediatR;

namespace Application.CQRS
{
    public record GetDashboardMetricsQuery()
    : IRequest<IReadOnlyList<DashboardMetricDto>>;
    public record GetTopProductsQuery(int Limit)
        : IRequest<IReadOnlyList<TopProductDto>>;

    public class DashboardHandlers(IInventoServices service) :
    IRequestHandler<GetDashboardMetricsQuery, IReadOnlyList<DashboardMetricDto>>,
    IRequestHandler<GetTopProductsQuery, IReadOnlyList<TopProductDto>>
    {
        private readonly IInventoServices _service = service;

        public async Task<IReadOnlyList<DashboardMetricDto>> Handle(
            GetDashboardMetricsQuery request, CancellationToken cancellationToken)
        {
            var metrics = await _service.DashboardService.GetMetricsAsync(cancellationToken);
               return [.. metrics.Select(m => new DashboardMetricDto(m.Name, m.Value, m.Unit))];
        }


        public async Task<IReadOnlyList<TopProductDto>> Handle(
            GetTopProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _service.DashboardService.GetTopProductsAsync(request.Limit, cancellationToken);
            return [.. products.Select(p => new TopProductDto(p.ProductId,p.ProductName,p.TotalSoldQuantity))];
        }
    }


}
