using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Entities;

namespace RestaurantApi.Controllers;

[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    private RestaurantDbContext _context;

    public RestaurantController(RestaurantDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Restaurant>> GetAll()
    {
        var restaurants = _context.Restaurants.ToList();
        return Ok(restaurants);
    }
    [HttpGet]
    [Route("{id}")]
    public ActionResult<Restaurant> GetById(int id)
    {
        var restaurant = _context.Restaurants.Find(id);
        if (restaurant != null)
        {
            return Ok(restaurant);
        }

        return NotFound();
    }
}