using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Models;
using AutoMapper;
using RestaurantApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RestaurantApi.Authorization;

namespace RestaurantApi.Services;

public interface IRestaurantService
{
    int Create(CreateRestaurantDto dto, int userId);
    IEnumerable<RestaurantDto> GetAll();
    RestaurantDto GetById(int id);
    void Delete(int id, ClaimsPrincipal user);
    void Edit(int id, EditRestaurantDto dto, ClaimsPrincipal user);
}

public class RestaurantService : IRestaurantService
{
    private RestaurantDbContext _context;
    private IMapper _mapper;
    private ILogger<RestaurantService> _logger;
    private IAuthorizationService _authorizationService;

    public RestaurantService(RestaurantDbContext context, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _authorizationService = authorizationService;
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

    public int Create(CreateRestaurantDto dto, int userId)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);
        restaurant.CreatedById = userId;
        _context.Restaurants.Add(restaurant);
        _context.SaveChanges();

        return restaurant.Id;
    }

    public void Delete(int id, ClaimsPrincipal user)
    {
        _logger.LogError($"Restaurant with id {id} - DELETE action invoked.");

        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id {id} not found");
        }

        var authResult = _authorizationService.AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
        if (!authResult.Succeeded) throw new ForbiddenException();

        _context.Restaurants.Remove(restaurant);
        _context.SaveChanges();
    }

    public void Edit(int id, EditRestaurantDto dto, ClaimsPrincipal user)
    {

        var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant with id {id} not found");
        }

        var authResult = _authorizationService.AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
        if (!authResult.Succeeded) throw new ForbiddenException();

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.HasDelivery = (bool)dto.HasDelivery!;

        _context.SaveChanges();
    }
}