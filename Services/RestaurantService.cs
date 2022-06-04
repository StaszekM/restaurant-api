using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Models;
using AutoMapper;

namespace RestaurantApi.Services;

public interface IRestaurantService
{
    int Create(CreateRestaurantDto dto);
    IEnumerable<RestaurantDto> GetAll();
    RestaurantDto? GetById(int id);
    bool Delete(int id);
    bool Edit(int id, EditRestaurantDto dto);
}

public class RestaurantService : IRestaurantService
{
    private RestaurantDbContext _context;
    private IMapper _mapper;

    public RestaurantService(RestaurantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public RestaurantDto? GetById(int id)
    {
        var restaurant = _context.Restaurants
                           .Include(r => r.Address)
                           .Include(r => r.Dishes)
                           .FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            return null;
        }

        return _mapper.Map<RestaurantDto>(restaurant);
    }

    public IEnumerable<RestaurantDto> GetAll()
    {
        var restaurants = _context.Restaurants
                            .Include(r => r.Address)
                            .Include(r => r.Dishes)
                            .ToList();
        var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

        return restaurantDtos;
    }

    public int Create(CreateRestaurantDto dto)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);
        _context.Restaurants.Add(restaurant);
        _context.SaveChanges();

        return restaurant.Id;
    }

    public bool Delete(int id)
    {
        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            return false;
        }

        _context.Restaurants.Remove(restaurant);
        _context.SaveChanges();

        return true;
    }

    public bool Edit(int id, EditRestaurantDto dto)
    {
        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            return false;
        }

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.HasDelivery = (bool) dto.HasDelivery!;
        
        _context.SaveChanges();

        return true;
    }
}