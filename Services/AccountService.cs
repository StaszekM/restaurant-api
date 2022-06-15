using RestaurantApi.Entities;
using RestaurantApi.Models;

namespace RestaurantApi.Services;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto registerUserDto);
}

public class AccountService : IAccountService
{
    private readonly RestaurantDbContext _context;
    public AccountService(RestaurantDbContext context)
    {
        _context = context;
    }

    public void RegisterUser(RegisterUserDto registerUserDto)
    {
        var newUser = new User()
        {
            Email = registerUserDto.Email,
            DateOfBirth = registerUserDto.DateOfBirth,
            Nationality = registerUserDto.Nationality,
            RoleId = registerUserDto.RoleId
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();
    }
}