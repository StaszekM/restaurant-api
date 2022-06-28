namespace RestaurantApi.Models;

public class RegisterUserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmedPassword { get; set; } = null!;
    public string? Nationality { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int RoleId { get; set; } = 1;
}