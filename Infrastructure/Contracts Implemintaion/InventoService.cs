using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;

using authservcie;

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
    public class InventoService(IMapper mapper, IUnitOfWork unitOfWork, IDistributedCache cache, ApplicationDbContext context, IExternalApisService externalApisService,
        IConfiguration config,
        IFluentEmail email,
        IMemoryCache memoryCache
            ) : IInventoServices
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IDistributedCache _cache = cache;
        private readonly ApplicationDbContext _context = context;
        private readonly IExternalApisService _externalApisService = externalApisService;
        private readonly IConfiguration _config = config;
        private readonly IFluentEmail _email = email;
        private readonly IMemoryCache _memoryCache = memoryCache;

        public ICategoryService categoryService =>  new CategoryService(_mapper, _unitOfWork);

        public IProductService ProductService =>   new ProductcService(_mapper, _unitOfWork);

        public ISupplierService SupplierService =>  new SupplierService(_mapper, _unitOfWork,_externalApisService);

        public IDashboardService DashboardService => new DashboardService(_context, _cache);

        public IStockService StockService => new StockService(_mapper, _unitOfWork);

        public IAuthService AuthService =>  new AuthService(_config,_unitOfWork,_email, _memoryCache);
    }
}
