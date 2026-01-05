using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;
using Application.InternalServices;

using AutoMapper;

using Domain.Entites;

using Infrastructure.IRepos;

namespace Application.Internal_Services_implementation
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }
    }
}

