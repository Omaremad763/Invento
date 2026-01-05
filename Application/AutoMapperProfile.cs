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
            // هنا تعمل جميع الـ DTOs ↔ Entities
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
        }
    }
}
