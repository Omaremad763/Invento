using Application.Contracts;
using Application.DTOS;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Domain.Entites;

using Infrastructure.Extentions;

namespace Application.Internal_Services_implementation
{
    public class SupplierService(IMapper mapper, IUnitOfWork unitOfWork, IExternalApisService externalApisService) : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IExternalApisService _externalApisService = externalApisService;

        public async Task<PaginatedResult<SupplierDto>> GetAllSuppliersAsync(ResourceParameters Parameters)
        {
            var suppliers = _unitOfWork.Suppliers.GetAllAsync();

            if (!string.IsNullOrEmpty(Parameters.SearchTerm))
            {
                var search = Parameters.SearchTerm.Trim().ToLower();
                suppliers = suppliers.Where(p => p.Name.ToLower().Contains(search) || p.ContactEmail.ToLower().Contains(search));
            }

            var projectedQuery = suppliers.ProjectTo<SupplierDto>(_mapper.ConfigurationProvider);

            var result = await projectedQuery.ToPaginatedListAsync(Parameters.PageNumber, Parameters.PageSize);
            return result;
        }

        public async Task<(bool, string)> AddSupplierAsync(AddSupplierDto DTO, string CountryCode, string VatNumber)
        {
            var (IsValid, CompanyName) = await _externalApisService.ValidateVatAsync(CountryCode, VatNumber);

            if (!IsValid)
            {
                return (false, "UntrustedCompany");
            }
            var Mapping = _mapper.Map<Supplier>(DTO);
            Mapping.Vatstatus = "verified";
            await _unitOfWork.Suppliers.AddAsync(Mapping);
            int saving = await _unitOfWork.CommitAsync();
            return saving > 0 ? (true, "Supplier saved") : (false, "Failed to save supplier");
        }

        public async Task<bool> UpdateSupplierAsync(UpdateSupplierDto dto)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.Id);
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