using AutoMapper;
using EazyMenu.Application.Common.Models.Auth;
using EazyMenu.Application.Common.Models.Order;
using EazyMenu.Application.Common.Models.Restaurant;
using EazyMenu.Domain.Entities;

namespace EazyMenu.Application.Common.Mappings;

/// <summary>
/// پروفایل Mapping برای AutoMapper
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Restaurant Mappings
        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.Ignore()); // Set manually in handler

        CreateMap<Restaurant, RestaurantListDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.Ignore()); // Set manually in handler

        // Order Mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name))
            .ForMember(dest => dest.RestaurantPhone, opt => opt.MapFrom(src => src.Restaurant.PhoneNumber))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderItem, OrderItemDto>();

        // Commands mapping (if needed in future)
        // CreateMap<CreateRestaurantDto, CreateRestaurantCommand>();
        // CreateMap<UpdateRestaurantDto, UpdateRestaurantCommand>();

        // ApplicationUser Mappings
        CreateMap<ApplicationUser, UserInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
            .ForMember(dest => dest.PreferredLanguage, opt => opt.MapFrom(src => src.PreferredLanguage))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        // Reservation Mappings
        CreateMap<Reservation, Common.Models.Reservation.ReservationDto>()
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : ""))
            .ForMember(dest => dest.RestaurantPhone, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.PhoneNumber : ""))
            .ForMember(dest => dest.RestaurantAddress, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Address : ""));

        CreateMap<Reservation, Common.Models.Reservation.ReservationListDto>()
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : ""));
    }
}
