using RestaurantApi.Models;
using RestaurantApi.Entities;
public class RestaurantMappingProfile : AutoMapper.Profile
{
    public RestaurantMappingProfile()
    {
        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
            .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
            .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

        CreateMap<Dish, DishDto>();

        CreateMap<CreateRestaurantDto, Restaurant>()
            .ForMember(m => m.Address, c => c.MapFrom(dto => new Address { City = dto.City, PostalCode = dto.PostalCode, Street = dto.Street }));
    }
}