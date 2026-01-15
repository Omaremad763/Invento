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
    //Queries
    public record GetDashboardMetricsQuery()
    : IRequest<IReadOnlyList<DashboardMetricDto>>;
    public record GetTopProductsQuery(int Limit)
        : IRequest<IReadOnlyList<TopProductDto>>;

    public class DashboardHandlers :
    IRequestHandler<GetDashboardMetricsQuery, IReadOnlyList<DashboardMetricDto>>,
    IRequestHandler<GetTopProductsQuery, IReadOnlyList<TopProductDto>>
    {
        private readonly IDashboardService _service;
        public DashboardHandlers(IDashboardService service)
        {
            _service = service;
        }

        // Handler 1: Metrics
        public async Task<IReadOnlyList<DashboardMetricDto>> Handle(
            GetDashboardMetricsQuery request, CancellationToken ct)
        {
            var metrics = await _service.GetMetricsAsync(ct);
               return metrics
              .Select(m => new DashboardMetricDto(m.Name, m.Value, m.Unit)).ToList();
        }


        // Handler 2: Top Products
        public async Task<IReadOnlyList<TopProductDto>> Handle(
            GetTopProductsQuery request, CancellationToken ct)
        {
            var products = await _service.GetTopProductsAsync(request.Limit, ct);
            return products.Select(p => new TopProductDto(p.ProductId,p.ProductName,p.TotalSoldQuantity)).ToList();
        }
    }


}
