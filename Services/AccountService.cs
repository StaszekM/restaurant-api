using Microsoft.AspNetCore.Identity;
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
    private readonly IPasswordHasher<User> _hasher;
    public AccountService(RestaurantDbContext context, IPasswordHasher<User> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public void RegisterUser(RegisterUserDto registerUserDto)
    {
        var newUser = new User()
        {
            Email = registerUserDto.Email,
            DateOfBirth = registerUserDto.DateOfBirth,
            Nationality = registerUserDto.Nationality,
            RoleId = registerUserDto.RoleId,
        };

        newUser.PasswordHash = _hasher.HashPassword(newUser, registerUserDto.Password);
        _context.Users.Add(newUser);
        _context.SaveChanges();
    }
}