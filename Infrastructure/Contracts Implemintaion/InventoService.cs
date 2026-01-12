using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;

using AutoMapper;

using Infrastructure.Persistence;
using Infrastructure.Repos;

using Microsoft.Extensions.Caching.Distributed;

namespace Application.Internal_Services_implementation
{
    public class InventoService : IInventoServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ApplicationDbContext _context;

        public InventoService(IMapper mapper, IUnitOfWork unitOfWork, IDistributedCache cache, ApplicationDbContext context)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cache= cache;
            _context = context;
        }
        public ICategoryService categoryService =>  new CategoryService(_mapper, _unitOfWork);

        public IProductService ProductService =>   new ProductcService(_mapper, _unitOfWork);

        public ISupplierService SupplierService =>  new SupplierService(_mapper, _unitOfWork);

        public IDashboardService DashboardService => new DashboardService(_context, _cache);
    }
}
