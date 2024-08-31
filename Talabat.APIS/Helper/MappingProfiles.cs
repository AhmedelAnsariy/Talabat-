using AutoMapper;
using Talabat.APIS.DTO;
using Talabat.Core.Entities;
using  UserAddress =  Talabat.Core.Entities.Identity.Address;
using  OrderAddress =  Talabat.Core.Entities.Order.Address;
using Talabat.Core.Entities.Order;

namespace Talabat.APIS.Helper
{ 
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
             .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name))
             .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
             .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());



            CreateMap<UserAddress, AddressDto>().ReverseMap();


            CreateMap<OrderAddress, AddressDto>().ReverseMap()
                   .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FName))
                   .ForMember(d => d.LastName, o => o.MapFrom(s => s.LName));


            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethods, o => o.MapFrom(s => s.DeliveryMethods.ShortName))
                .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethods.Cost));



            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());



            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();

        }
    }
}
