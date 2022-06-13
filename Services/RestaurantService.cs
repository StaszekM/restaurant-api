using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Models;
using AutoMapper;
using RestaurantApi.Exceptions;

namespace RestaurantApi.Services;

public interface IRestaurantService
{
    int Create(CreateRestaurantDto dto);
    IEnumerable<RestaurantDto> GetAll();
    RestaurantDto GetById(int id);
    void Delete(int id);
    void Edit(int id, EditRestaurantDto dto);
}

public class RestaurantService : IRestaurantService
{
    private RestaurantDbContext _context;
    private IMapper _mapper;
    private ILogger<RestaurantService> _logger;

    public RestaurantService(RestaurantDbContext context, IMapper mapper, ILogger<RestaurantService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public RestaurantDto GetById(int id)
    {
        var restaurant = _context.Restaurants
                           .Include(r => r.Address)
                           .Include(r => r.Dishes)
                           .FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id {id} not found");
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

    public void Delete(int id)
    {
        _logger.LogError($"Restaurant with id {id} - DELETE action invoked.");

        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id {id} not found");
        }

        _context.Restaurants.Remove(restaurant);
        _context.SaveChanges();
    }

    public void Edit(int id, EditRestaurantDto dto)
    {
        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id {id} not found");
        }

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.HasDelivery = (bool)dto.HasDelivery!;

        _context.SaveChanges();
    }
}