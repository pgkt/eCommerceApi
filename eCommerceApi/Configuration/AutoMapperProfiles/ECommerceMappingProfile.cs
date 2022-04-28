using AutoMapper;
using eCommerceApi.Domain.Entities;
using eCommerceApi.DTOs;
using eCommerceApi.ViewModels;

namespace eCommerceApi.Configuration.AutoMapperProfiles
{
    public class ECommerceMappingProfile : Profile
    {
        public ECommerceMappingProfile()
        {
            CreateMap<MerchantDto, Merchant>();
            CreateMap<Merchant, MerchantViewModel>();

            CreateMap<ProductDto, Product>();
            CreateMap<Product, ProductViewModel>();

            CreateMap<ProductCategoryDto, ProductCategory>();
            CreateMap<ProductCategory, ProductCategoryDto>();

            CreateMap<OrderDto, Order>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemViewModel>();

            CreateMap<Merchant, User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.MerchantId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.MerchantName));
        }
    }
}
