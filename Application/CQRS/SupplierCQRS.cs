using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;
using Application.DTOS.Update_DTOS;

using Domain.Entites;

using FluentValidation;

using MediatR;

namespace Application.CQRS;

//Queries
public record GetSuppliersQuery(ResourceParameters Parameters) : IRequest<PaginatedResult<SupplierDto>>;

//commands
public record AddSupplierCommand(AddSupplierDTO SupplierDTO, string CountryCode, string VatNumber) : IRequest<(bool, string)>;
public record UpdateSupplierCommand(UpdateSupplierDTO UpdateSupplierDTO) : IRequest<bool>;
public record DeleteSupplierCommand(Guid Id) : IRequest<bool>;


//validators

public class AddSupplierValidator : AbstractValidator<AddSupplierCommand>
{
    public AddSupplierValidator()
    {
        RuleFor(x => x.SupplierDTO.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SupplierDTO.ContactEmail).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SupplierDTO.PhoneNumber).NotEmpty().MaximumLength(12);
    }
}
public class UpdateSupplierValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierValidator()
    {
        RuleFor(x => x.UpdateSupplierDTO.id).NotEqual(Guid.Empty)
            .WithMessage("Supplier ID is required for updates.");
    }
}

public class DeleteSupplierValidator : AbstractValidator<DeleteSupplierCommand>
{
    public DeleteSupplierValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty)
            .WithMessage("A valid Supplier ID must be provided.");
    }
}
//handleres
public class SupplierHandlers :
            IRequestHandler<GetSuppliersQuery, PaginatedResult<SupplierDto>>,
            IRequestHandler<AddSupplierCommand, (bool,string)>,
            IRequestHandler<UpdateSupplierCommand, bool>,
            IRequestHandler<DeleteSupplierCommand, bool>

{
    private readonly IInventoServices _service;
    private readonly IExternalApisService _externalApisService;
    public SupplierHandlers(IInventoServices service, IExternalApisService externalApisService)
    { _service = service; _externalApisService = externalApisService; }


    public async Task<PaginatedResult<SupplierDto>> Handle(GetSuppliersQuery req, CancellationToken ct)
        => await _service.SupplierService.GetAllSuppliersAsync(req.Parameters);

    public async Task<(bool,string)> Handle(AddSupplierCommand req, CancellationToken ct)
=> await _service.SupplierService.AddSupplierAsync(req.SupplierDTO,req.CountryCode,req.VatNumber);

    public async Task<bool> Handle(UpdateSupplierCommand req, CancellationToken ct)
=> await _service.SupplierService.UpdateSupplierAsync(req.UpdateSupplierDTO);

    public async Task<bool> Handle(DeleteSupplierCommand req, CancellationToken ct)
=> await _service.SupplierService.SoftDeleteSupplierAsync(req.Id);
}
