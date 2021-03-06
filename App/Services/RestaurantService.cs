using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Models;
using AutoMapper;
using RestaurantApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi.Authorization;
using System.Linq.Expressions;

namespace RestaurantApi.Services;
public interface IRestaurantService
{
    int Create(CreateRestaurantDto dto);
    void Delete(int id);
    void Edit(int id, EditRestaurantDto dto);
    PageResult<RestaurantDto> GetAll(RestaurantQuery query);
    RestaurantDto GetById(int id);
}

public class RestaurantService : IRestaurantService
{
    private RestaurantDbContext _context;
    private IMapper _mapper;
    private ILogger<RestaurantService> _logger;
    private IAuthorizationService _authorizationService;
    private IUserContextService _userContextService;

    public RestaurantService(
        RestaurantDbContext context,
        IMapper mapper,
        ILogger<RestaurantService> logger,
        IAuthorizationService authorizationService,
        IUserContextService userContextService)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _authorizationService = authorizationService;
        _userContextService = userContextService;
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

    public PageResult<RestaurantDto> GetAll(RestaurantQuery query)
    {
        var searchPhrase = query.SearchPhrase;
        var pageSize = query.PageSize;
        var pageNumber = query.PageNumber;

        var restaurantsUnpaginated = _context.Restaurants
                            .Include(r => r.Address)
                            .Include(r => r.Dishes)
                            .Where(r =>
                            string.IsNullOrEmpty(searchPhrase) ||
                            r.Name.ToLower().Contains(searchPhrase.ToLower()) ||
                            r.Description.ToLower().Contains(searchPhrase.ToLower()));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>() {
                {nameof(Restaurant.Name), r => r.Name},
                {nameof(Restaurant.Description), r => r.Description},
                {nameof(Restaurant.Category), r => r.Category}
            };
            var selectedColumn = columnsSelectors[query.SortBy];
            restaurantsUnpaginated = query.SortDirection == SortDirection.ASC ? restaurantsUnpaginated.OrderBy(selectedColumn) : restaurantsUnpaginated.OrderByDescending(selectedColumn);
        }

        List<Restaurant> restaurants = restaurantsUnpaginated.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

        var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

        return new PageResult<RestaurantDto>(restaurantDtos, restaurantsUnpaginated.Count(), pageSize, pageNumber);
    }

    public int Create(CreateRestaurantDto dto)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);
        restaurant.CreatedById = _userContextService.GetUserId();
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

        var authResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
        if (!authResult.Succeeded) throw new ForbiddenException();

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

        var authResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
        if (!authResult.Succeeded) throw new ForbiddenException();

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.HasDelivery = (bool)dto.HasDelivery!;

        _context.SaveChanges();
    }
}