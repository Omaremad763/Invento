using Application.DTOS;

using AutoMapper;

using Domain.Entites;

namespace Application;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Products

        CreateMap<Product, GetProductsDto>().ForCtorParam("CategoryName",
opt => opt.MapFrom(src => src.Category.CategoryName))
    .ForCtorParam("id", opt => opt.MapFrom(src => src.Id));
        CreateMap<AddProductDto, Product>();
        CreateMap<UpdateProductDto, Product>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
        {
            if (srcMember is string stringValue && string.IsNullOrWhiteSpace(stringValue)) return false;
            if (srcMember is Guid g && g == Guid.Empty) return false;
            return true;
        }));

        CreateMap<Product, GetProductsLookUpDto>();

        #endregion Products

        CreateMap<CategoryDto, Category>().ReverseMap();

        #region suppliers

        CreateMap<SupplierDto, Supplier>().ReverseMap();
        CreateMap<UpdateSupplierDto, Supplier>()
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
        {
            if (srcMember is string stringValue && string.IsNullOrWhiteSpace(stringValue)) return false;
            if (srcMember is Guid g && g == Guid.Empty) return false;
            return true;
        }));
        CreateMap<AddSupplierDto, Supplier>();

        #endregion suppliers

        CreateMap<StockTransaction, GetStockTransactionDto>()
            .ForCtorParam("ProductName", opt => opt.MapFrom(src => src.Product.Name))
            .ForCtorParam("StockTransactionType", opt => opt.MapFrom(src => src.StockTransactionType.ToString()))
            .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id));

        CreateMap<AddStockTransactionDto, StockTransaction>();
    }
}