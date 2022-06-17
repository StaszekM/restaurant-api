using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantApi.Entities;
using RestaurantApi.Exceptions;
using RestaurantApi.Models;

namespace RestaurantApi.Services;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto registerUserDto);
    string GenerateJwt(LoginDto loginDto);
}

public class AccountService : IAccountService
{
    private readonly RestaurantDbContext _context;
    private readonly IPasswordHasher<User> _hasher;
    private readonly AuthenticationSettings _authenticationSettings;
    public AccountService(RestaurantDbContext context, IPasswordHasher<User> hasher, AuthenticationSettings authenticationSettings)
    {
        _context = context;
        _hasher = hasher;
        _authenticationSettings = authenticationSettings;
    }

    public string GenerateJwt(LoginDto loginDto)
    {
        var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == loginDto.Email);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, $"{user.Role.Name}")
        };

        if (!string.IsNullOrEmpty(user.Nationality))
        {
            claims.Add(new Claim("Nationality", user.Nationality));
        }

        if (user.DateOfBirth is not null)
        {
            claims.Add(new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyy-MM-dd")));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

        JwtSecurityToken token = new JwtSecurityToken(
            _authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
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