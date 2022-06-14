using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Exceptions;
using RestaurantApi.Models;

namespace RestaurantApi.Services;

public interface IDishService
{
    public int Create(int restaurantId, CreateDishDto createDishDto);
    DishDto GetById(int restaurantId, int dishId);
    List<DishDto> GetAll(int restaurantId);
    void DeleteAll(int restaurantId);
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
            throw new NotFoundException($"Restaurant with id {restaurantId} not found");

        var dishEntity = _mapper.Map<Dish>(createDishDto);

        dishEntity.RestaurantId = restaurantId;
        _context.Dishes.Add(dishEntity);
        _context.SaveChanges();

        return dishEntity.Id;
    }

    public DishDto GetById(int restaurantId, int dishId)
    {
        var restaurant = _context.Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.Id == restaurantId);

        if (restaurant is null)
            throw new NotFoundException($"Restaurant with id {restaurantId} not found");


        var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == dishId);

        if (dish is null || dish.RestaurantId != restaurantId)
            throw new NotFoundException($"Dish with id {dishId} not found");

        return _mapper.Map<DishDto>(dish);
    }

    public List<DishDto> GetAll(int restaurantId)
    {
        var restaurant = _context.Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.Id == restaurantId);

        if (restaurant is null)
            throw new NotFoundException($"Restaurant with id {restaurantId} not found");

        var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
        return dishDtos;
    }

    public void DeleteAll(int restaurantId)
    {
        var restaurant = _context.Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.Id == restaurantId);

        if (restaurant is null)
            throw new NotFoundException($"Restaurant with id {restaurantId} not found");

        _context.RemoveRange(restaurant.Dishes);
        _context.SaveChanges();
    }
}