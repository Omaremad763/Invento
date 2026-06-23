namespace Application.Contracts
{
    public interface IInventoServices
    {
        public ICategoryService categoryService { get; }
        public IProductService ProductService { get; }
        public ISupplierService SupplierService { get; }

        public IDashboardService DashboardService { get; }
        public IStockService StockService { get; }
        public IAuthService AuthService { get; }
    }
}