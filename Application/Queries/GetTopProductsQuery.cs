using Application.DTOS;

using MediatR;

namespace Application.Queries
{
    public record GetTopProductsQuery(int Limit)
        : IRequest<IReadOnlyList<TopProductDto>>;
}
