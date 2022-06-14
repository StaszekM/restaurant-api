using AutoMapper;
using RestaurantApi.Entities;
using RestaurantApi.Exceptions;
using RestaurantApi.Models;

namespace RestaurantApi.Services;

public interface IDishService
{
    public int Create(int restaurantId, CreateDishDto createDishDto);
}

public class DishService : IDishService
{
    private readonly RestaurantDbContext _context;
    private readonly IMapper _mapper;

    public DishService(RestaurantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public int Create(int restaurantId, CreateDishDto createDishDto)
    {
        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == restaurantId);
        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id {restaurantId} not found");
        }

        var dishEntity = _mapper.Map<Dish>(createDishDto);

        dishEntity.RestaurantId = restaurantId;
        _context.Dishes.Add(dishEntity);
        _context.SaveChanges();

        return dishEntity.Id;
    }
}