/*
 🎯 سبب التعديل (Refactoring Reason):
 --------------------------------------
 الكود القديم كان بيستخدم السهم (=>) مباشرة مع (new)، وده معناه إن كل سطر في الـ API بينده مثلاً (ProductService) 
 كان السي شارب بيجبر الميموري تخلق كائن جديد تماماً (New Instance) وتطرد القديم، حتى لو في نفس الـ Request!
 ده كان بيعمل ضغط عالي على الـ Garbage Collector، والأخطر إنه بيبوظ الـ State بتاعة الـ Unit of Work والـ DbContext.
 
 الحل الجديد:
 ------------
 زودنا متغيرات private (Backing Fields) فوق كل خدمة، واستخدمنا المعامل (??=). 
 معناه: أول نداء للخدمة جوه الـ Request هيلاقيها بـ null، فيروح يعمل لها (new) ويخزنها في المتغير الـ private.
 النداء الثاني والثالث في نفس الـ Request هيرجع النسخة المتخزنة الجاهزة فوراً بدون إعادة التخليق.
*/

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

        // 🔒 الـ Backing Fields لحفظ الحالات (State Caching)
        private ICategoryService _categoryService;
        private IProductService _productService;
        private ISupplierService _supplierService;
        private IDashboardService _dashboardService;
        private IStockService _stockService;
        private IAuthService _authService;

        // 🎯 بوابات الـ Facade الذكية مع الحفاظ على كائن واحد طول الـ Request
        public ICategoryService categoryService => _categoryService ??= new CategoryService(_mapper, _unitOfWork);

        public IProductService ProductService => _productService ??= new ProductcService(_mapper, _unitOfWork);

        public ISupplierService SupplierService => _supplierService ??= new SupplierService(_mapper, _unitOfWork, _externalApisService);

        public IDashboardService DashboardService => _dashboardService ??= new DashboardService(_context, _cache);

        public IStockService StockService => _stockService ??= new StockService(_mapper, _unitOfWork);

        public IAuthService AuthService => _authService ??= new AuthService(_config, _unitOfWork, _email, _memoryCache);
    }
}

