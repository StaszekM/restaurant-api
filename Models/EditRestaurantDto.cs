using System.ComponentModel.DataAnnotations;
namespace RestaurantApi.Models;

public class EditRestaurantDto
{
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    public bool? HasDelivery { get; set; }
}