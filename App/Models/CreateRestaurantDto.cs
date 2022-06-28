using System.ComponentModel.DataAnnotations;
namespace RestaurantApi.Models;

public class CreateRestaurantDto
{
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    [Required]
    public bool? HasDelivery { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactNumber { get; set; }
    [Required]
    [MaxLength(50)]
    public string City { get; set; } = null!;
    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = null!;
    public string? PostalCode { get; set; }
}