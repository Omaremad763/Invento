using Application.Contracts;
using Application.DTOS;

using FluentValidation;

using MediatR;

namespace Application.CQRS;

//Queries
public record GetCategoriesQuery(ResourceParameters Parameters) : IRequest<PaginatedResult<CategoryDto>>;

//commands
public record AddCategoryCommand(string CategoryName) : IRequest<bool>;
public record UpdateCategoryCommand(CategoryDto Category) : IRequest<bool>;
public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;

//validators

public class AddCateogryValidator : AbstractValidator<AddCategoryCommand>
{
    public AddCateogryValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(100);
    }
}

public class UpdateCateogryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCateogryValidator()
    {
        RuleFor(x => x.Category.Id).NotEqual(Guid.Empty)
            .WithMessage("Category ID is required for updates.");
    }
}

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty)
            .WithMessage("A valid Category ID must be provided.");
    }
}

//handleres
public class CategoryHandlers(IInventoServices service) :
            IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryDto>>,
            IRequestHandler<AddCategoryCommand, bool>,
            IRequestHandler<UpdateCategoryCommand, bool>,
            IRequestHandler<DeleteCategoryCommand, bool>

{
    private readonly IInventoServices _service = service;

    public async Task<PaginatedResult<CategoryDto>> Handle(GetCategoriesQuery req, CancellationToken cancellationToken)
            => await _service.categoryService.GetAllCategoriesAsync(req.Parameters);

    public async Task<bool> Handle(AddCategoryCommand req, CancellationToken cancellationToken)
=> await _service.categoryService.AddCategoryAsync(req.CategoryName);

    public async Task<bool> Handle(UpdateCategoryCommand req, CancellationToken cancellationToken)
=> await _service.categoryService.UpdateCategoryAsync(req.Category);

    public async Task<bool> Handle(DeleteCategoryCommand req, CancellationToken cancellationToken)
=> await _service.categoryService.SoftDeleteCategoryAsync(req.Id);
}