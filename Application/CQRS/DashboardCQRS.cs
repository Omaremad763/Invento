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

    public class DashboardHandlers :
    IRequestHandler<GetDashboardMetricsQuery, IReadOnlyList<DashboardMetricDto>>,
    IRequestHandler<GetTopProductsQuery, IReadOnlyList<TopProductDto>>
    {
        private readonly IInventoServices _service;
        public DashboardHandlers(IInventoServices service)
        {
            _service = service;
        }

        public async Task<IReadOnlyList<DashboardMetricDto>> Handle(
            GetDashboardMetricsQuery request, CancellationToken ct)
        {
            var metrics = await _service.DashboardService.GetMetricsAsync(ct);
               return metrics
              .Select(m => new DashboardMetricDto(m.Name, m.Value, m.Unit)).ToList();
        }


        public async Task<IReadOnlyList<TopProductDto>> Handle(
            GetTopProductsQuery request, CancellationToken ct)
        {
            var products = await _service.DashboardService.GetTopProductsAsync(request.Limit, ct);
            return products.Select(p => new TopProductDto(p.ProductId,p.ProductName,p.TotalSoldQuantity)).ToList();
        }
    }


}
