using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Models;
using RestaurantApi.Services;

namespace RestaurantApi.Controllers;

[Route("api/restaurant")]
public class RestaurantController : ControllerBase
{
    private IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService service)
    {
        _restaurantService = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<RestaurantDto>> GetAll()
    {
        var restaurantDtos = _restaurantService.GetAll();
        return Ok(restaurantDtos);
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<RestaurantDto> GetById(int id)
    {
        var restaurant = _restaurantService.GetById(id);

        if (restaurant is null)
        {
            return NotFound();
        }

        return Ok(restaurant);
    }

    [HttpPost]
    public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var id = _restaurantService.Create(dto);

        return Created($"/api/restaurant/{id}", null);
    }
    [HttpDelete("{id}")]
    public ActionResult Delete([FromRoute] int id)
    {
        var deleted = _restaurantService.Delete(id);

        if (deleted)
        {
            return NoContent();
        }

        return NotFound();
    }
}