using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;
using Application.DTOS.Update_DTOS;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Domain.Entites;

using Infrastructure.Extentions;

namespace Application.Internal_Services_implementation
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExternalApisService _externalApisService;

        public SupplierService(IMapper mapper, IUnitOfWork unitOfWork, IExternalApisService externalApisService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _externalApisService = externalApisService;
        }
        public async Task<PaginatedResult<SupplierDto>> GetAllSuppliersAsync(ResourceParameters Parameters)
        {
            var suppliers =  _unitOfWork.Suppliers.GetAllAsync();

            if (!string.IsNullOrEmpty(Parameters.SearchTerm))
            {
                var search = Parameters.SearchTerm.Trim().ToLower();
                suppliers = suppliers.Where(p => p.Name.ToLower().Contains(search)||p.ContactEmail.ToLower().Contains(search));
            }

            var projectedQuery = suppliers.ProjectTo<SupplierDto>(_mapper.ConfigurationProvider);

            var result = await projectedQuery.ToPaginatedListAsync(Parameters.PageNumber, Parameters.PageSize);
            return result;
        }

        public async Task<(bool,string)> AddSupplierAsync(AddSupplierDTO DTO, string CountryCode, string VatNumber)
        {
            var validationSupplier =await _externalApisService.ValidateVatAsync(CountryCode, VatNumber);

            if (!validationSupplier.IsValid)
            {
                return (false, "UntrustedCompany");
            }
            var Mapping = _mapper.Map<Supplier>(DTO);
            Mapping.Vatstatus = "verified";
            await _unitOfWork.Suppliers.AddAsync(Mapping);
            int saving = await _unitOfWork.CommitAsync();
            return saving > 0?(true, "Supplier saved"): (false, "Failed to save supplier");
        }

        public async Task<bool> UpdateSupplierAsync(UpdateSupplierDTO dto)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.id);
            if (supplier == null)
            {
                return false;
            }
            _mapper.Map(dto, supplier);
            int saving = await _unitOfWork.CommitAsync();
            return saving > 0;
        }

        public async Task<bool> SoftDeleteSupplierAsync(Guid id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return false;
            }
            supplier.IsDeleted = true;
            int saving = await _unitOfWork.CommitAsync();
            return saving > 0;
        }

    }
}

