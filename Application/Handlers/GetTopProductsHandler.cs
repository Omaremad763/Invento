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
    public class GetTopProductsHandler
        : IRequestHandler<GetTopProductsQuery, IReadOnlyList<TopProductDto>>
    {
        private readonly IDashboardService _repository;

        public GetTopProductsHandler(IDashboardService repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<TopProductDto>> Handle(
            GetTopProductsQuery request,
            CancellationToken cancellationToken)
        {
            var products = await _repository.GetTopProductsAsync(
                request.Limit,
                cancellationToken);

            return products
                .Select(p => new TopProductDto(
                    p.ProductId,
                    p.ProductName,
                    p.TotalSoldQuantity))
                .ToList();
        }
    }
}