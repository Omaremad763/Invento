using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using MediatR;

namespace Application.CQRS;

//Queries
public record GetCategoriesQuery(ResourceParameters Parameters) : IRequest< PaginatedResult <CategoryDto>>;


//handleres
public class CategoryHandlers :
        IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryDto>>
{
    private readonly IInventoServices _service;
    public CategoryHandlers(IInventoServices service) => _service = service;

    public async Task<PaginatedResult<CategoryDto>> Handle(GetCategoriesQuery req, CancellationToken ct)
        => await _service.categoryService.GetAllCategoriesAsync(req.Parameters);
}