using Application.Contracts;
using Application.DTOS;

using FluentValidation;

using MediatR;

namespace Application.CQRS;

// Commands
public record AddProductCommand(AddProductDto Product) : IRequest<bool>;
public record DeleteProductCommand(Guid Id) : IRequest<bool>;
public record UpdateProductCommand(UpdateProductDto Product) : IRequest<bool>;
//Queries
public record GetProductByIDQuery(Guid SearchID) : IRequest<GetProductsDto>;
public record GetProductsQuery(ResourceParameters Parameters) : IRequest<PaginatedResult<GetProductsDto>>;

//fluent Validation

public class ProductDtoValidator : AbstractValidator<AddProductDto>
{
    public ProductDtoValidator()
    {
        // make rules for create only
        RuleSet("CreateOnly", () =>
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        });
        RuleFor(x => x.Price).GreaterThan(valueToCompare: 0);
    }
}

public class AddProductValidator : AbstractValidator<AddProductCommand>
{
    public AddProductValidator()
    {
        RuleFor(x => x.Product).SetValidator(new ProductDtoValidator(), "default", "CreateOnly");
    }
}

public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty)
            .WithMessage("A valid Product ID must be provided.");
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator() : base()
    {
        RuleFor(x => x.Product.Id).NotEqual(Guid.Empty)
            .WithMessage("Product ID is required for updates.");
    }
}

// Handlers
public class ProductHandlers(IInventoServices service) :
    IRequestHandler<GetProductsQuery, PaginatedResult<GetProductsDto>>,
    IRequestHandler<AddProductCommand, bool>,
    IRequestHandler<GetProductByIDQuery, GetProductsDto>,
    IRequestHandler<UpdateProductCommand, bool>,
    IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IInventoServices _service = service;

    public async Task<PaginatedResult<GetProductsDto>> Handle(GetProductsQuery req, CancellationToken cancellationToken)
            => await _service.ProductService.GetAllProductsAsync(req.Parameters);

    public async Task<GetProductsDto> Handle(GetProductByIDQuery request, CancellationToken cancellationToken)
    => await _service.ProductService.GetProductByIdAsync(request.SearchID);

    public async Task<bool> Handle(AddProductCommand req, CancellationToken cancellationToken)
            => await _service.ProductService.AddProductAsync(req.Product);

    public async Task<bool> Handle(UpdateProductCommand req, CancellationToken cancellationToken)
    => await _service.ProductService.UpdateProductAsync(req.Product);

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    => await _service.ProductService.SoftDeleteProductAsync(request.Id);
}