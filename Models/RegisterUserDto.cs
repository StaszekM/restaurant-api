using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Models;

public class RegisterUserDto
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    public string Nationality { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public int RoleId { get; set; } = 1;
}