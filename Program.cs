using RestaurantApi.Services;
using RestaurantApi.Entities;
using RestaurantApi.Middleware;
using NLog.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using FluentValidation;
using RestaurantApi.Models;
using RestaurantApi.Models.Validators;
using System.Text;
using RestaurantApi.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

// Add services to the container.

Console.WriteLine("Setting up services...");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasNationality", policyBuilder =>
    {
        policyBuilder.RequireClaim("Nationality");
    });
    options.AddPolicy("AtLeast20", policyBuilder =>
    {
        policyBuilder.AddRequirements(new MinimumAgeRequirement(20));
    });
    options.AddPolicy("AtLeast2Restaurants", policyBuilder =>
    {
        policyBuilder.AddRequirements(new MinRestaurantsCreatedRequirement(2));
    });
});
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>(); // I can add more handlers under this interface, DI will know what implementation is needed
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MinRestaurantsCreatedRequirementHandler>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<TimeTrackingMiddleware>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policyBuilder =>
    {
        policyBuilder.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:8080");
    });
});

Console.WriteLine("Setting up logging...");
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

Console.WriteLine("Building...");
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseResponseCaching();
app.UseCors("FrontendClient");
app.Services.CreateScope().ServiceProvider.GetService<RestaurantSeeder>()?.Seed();

app.UseMiddleware<TimeTrackingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RestaurantApi");
});

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Running...");
app.Run();

NLog.LogManager.Shutdown();