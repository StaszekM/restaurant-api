using Microsoft.AspNetCore.Mvc;
using Moq;
using RestaurantApi.Controllers;
using RestaurantApi.Exceptions;
using RestaurantApi.Models;
using RestaurantApi.Services;
using Xunit;
namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var mock = new Mock<IRestaurantService>();
        mock.Setup<RestaurantDto>(service => service.GetById(1)).Returns(new RestaurantDto() {Name = "asdf"});

        var controller = new RestaurantController(mock.Object);

        var result = controller.GetById(1);

        Assert.IsType<ActionResult<RestaurantDto>>(result);

    }
}