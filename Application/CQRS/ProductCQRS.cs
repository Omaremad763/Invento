using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using FluentValidation;

using MediatR;

namespace Application.CQRS;

    // Commands
    public record AddProductCommand(ProductDto Product) : IRequest<bool>;
    public record DeleteProductCommand(Guid Id) : IRequest<bool>;
    public record UpdateProductCommand(ProductDto Product) : IRequest<bool>;
    //Queries
    public record GetProductByIDQuery(Guid SearchID) : IRequest<ProductDto>;
    public record GetProductsQuery() : IRequest<IEnumerable<ProductDto>>;

//fluent Validation

    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            // make rules for create only
                RuleSet("CreateOnly", () =>
                {
                    RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                });
                RuleFor(x => x.Price).GreaterThan(0);
                RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
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
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty)
                .WithMessage("A valid Product ID must be provided.");
        }
    }

    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
        public UpdateProductValidator():base()
        {
            RuleFor(x => x.Product.Id).NotEmpty().NotEqual(Guid.Empty)
                .WithMessage("Product ID is required for updates.");
            RuleFor(x => x.Product).SetValidator(new ProductDtoValidator());
        }
    }
    // Handlers
    public class ProductHandlers :
        IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>,
        IRequestHandler<AddProductCommand, bool>,
        IRequestHandler<GetProductByIDQuery, ProductDto>,
        IRequestHandler<UpdateProductCommand, bool>,
        IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IInventoServices _service;
        public ProductHandlers(IInventoServices service) => _service = service;

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery req, CancellationToken ct)
            => await _service.ProductService.GetAllProductsAsync();
      public async Task<ProductDto> Handle(GetProductByIDQuery request, CancellationToken cancellationToken)
      => await _service.ProductService.GetProductByIdAsync(request.SearchID);

    public async Task<bool> Handle(AddProductCommand req, CancellationToken ct)
            => await _service.ProductService.AddProductAsync(req.Product);

        public async Task<bool> Handle(UpdateProductCommand req, CancellationToken ct)
        => await _service.ProductService.UpdateProductAsync(req.Product);

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        => await _service.ProductService.SoftDeleteProductAsync(request.Id);

    }

