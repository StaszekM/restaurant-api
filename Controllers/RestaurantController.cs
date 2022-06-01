using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Models;
using AutoMapper;

namespace RestaurantApi.Controllers;

[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    private RestaurantDbContext _context;
    private IMapper _mapper;

    public RestaurantController(RestaurantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<RestaurantDto>> GetAll()
    {
        var restaurants = _context.Restaurants
                            .Include(r => r.Address)
                            .Include(r => r.Dishes)
                            .ToList();
        var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

        return Ok(restaurantDtos);
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<RestaurantDto> GetById(int id)
    {
        var restaurant = _context.Restaurants
                            .Include(r => r.Address)
                            .Include(r => r.Dishes)
                            .FirstOrDefault(r => r.Id == id);
        if (restaurant != null)
        {
            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return Ok(restaurantDto);
        }

        return NotFound();
    }

    [HttpPost]
    public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);
        _context.Restaurants.Add(restaurant);
        _context.SaveChanges();

        return Created($"/api/restaurant/{restaurant.Id}", null);
    }
}