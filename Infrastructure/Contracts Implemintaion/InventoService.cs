using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;

using AutoMapper;

using FluentEmail.Core;

using Infrastructure.Contracts_Implemintaion;
using Infrastructure.Persistence;
using Infrastructure.Repos;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Application.Internal_Services_implementation
{
    public class InventoService : IInventoServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ApplicationDbContext _context;
        private readonly IExternalApisService _externalApisService;
        private readonly IConfiguration _config;
        private readonly IFluentEmail _email;
        private readonly IMemoryCache _memoryCache;


        public InventoService(IMapper mapper, IUnitOfWork unitOfWork, IDistributedCache cache, ApplicationDbContext context, IExternalApisService externalApisService,
            IConfiguration config,
            IFluentEmail email,
            IMemoryCache memoryCache
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cache= cache;
            _context = context;
            _externalApisService = externalApisService;
            _config = config;
            _email = email;
            _memoryCache = memoryCache;
        }
        public ICategoryService categoryService =>  new CategoryService(_mapper, _unitOfWork);

        public IProductService ProductService =>   new ProductcService(_mapper, _unitOfWork);

        public ISupplierService SupplierService =>  new SupplierService(_mapper, _unitOfWork,_externalApisService);

        public IDashboardService DashboardService => new DashboardService(_context, _cache);

        public IStockService StockService => new StockService(_mapper, _unitOfWork);

        public IAuthService AuthService =>  new AuthService(_config,_unitOfWork,_email, _memoryCache);
    }
}
