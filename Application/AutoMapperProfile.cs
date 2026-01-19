using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

using AutoMapper;

using Domain.Entites;

namespace Application
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, AddProductDto>().ReverseMap();
            CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
            {
                if (srcMember is string stringValue && string.IsNullOrWhiteSpace(stringValue))return false;
                if (srcMember is Guid g && g == Guid.Empty) return false;
                return true;
            }));
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
        }
    }
}
