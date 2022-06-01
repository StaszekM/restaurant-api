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
    }
}