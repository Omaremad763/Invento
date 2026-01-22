using Application.DTOS;

using AutoMapper;

using Domain.Entites;

namespace Application;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            {
                CreateMap<Product, GetProductsDTO>();
                CreateMap<AddProductDto, Product>();
                CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                {
                    if (srcMember is string stringValue && string.IsNullOrWhiteSpace(stringValue)) return false;
                    if (srcMember is Guid g && g == Guid.Empty) return false;
                    return true;
                }));
                CreateMap<CategoryDto, Category>().ReverseMap();
                CreateMap<Supplier, SupplierDto>().ReverseMap();
            }
        }
    }

