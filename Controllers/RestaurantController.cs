using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Models;
using RestaurantApi.Services;

namespace RestaurantApi.Controllers;

[Route("api/restaurant")]
[ApiController]
[Authorize]
public class RestaurantController : ControllerBase
{
    private IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    [Authorize(Policy = "AtLeast2Restaurants")]
    public ActionResult<IEnumerable<RestaurantDto>> GetAll()
    {
        var restaurantDtos = _restaurantService.GetAll();
        return Ok(restaurantDtos);
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<RestaurantDto> GetById(int id)
    {
        RestaurantDto restaurant = _restaurantService.GetById(id);
        return Ok(restaurant);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        var id = _restaurantService.Create(dto);
        return Created($"/api/restaurant/{id}", null);
    }
    [HttpDelete("{id}")]
    public ActionResult Delete([FromRoute] int id)
    {
        _restaurantService.Delete(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public ActionResult Edit([FromRoute] int id, [FromBody] EditRestaurantDto dto)
    {
        _restaurantService.Edit(id, dto);
        return NoContent();
    }
}