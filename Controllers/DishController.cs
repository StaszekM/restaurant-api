using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Models;
using RestaurantApi.Services;

namespace RestaurantApi.Controllers;

[Route("api/restaurant/{restaurantId}/dish")]
[ApiController]
public class DishController : ControllerBase
{
    private readonly IDishService _dishService;
    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpPost]
    public ActionResult Post([FromRoute] int restaurantId, [FromBody] CreateDishDto createDishDto)
    {
        int dishId = _dishService.Create(restaurantId, createDishDto);
        return Created($"api/restaurant/{restaurantId}/dish/{dishId}", null);
    }

    [Route("{dishId}")]
    [HttpGet]
    public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
    {
        var dish = _dishService.GetById(restaurantId, dishId);
        return dish;
    }

    [HttpGet]
    public List<DishDto> GetAll([FromRoute] int restaurantId) {
        var dishes = _dishService.GetAll(restaurantId);
        return dishes;
    }

    [HttpDelete]
    public ActionResult Delete([FromRoute] int restaurantId) {
        _dishService.DeleteAll(restaurantId);
        return NoContent();
    }
}