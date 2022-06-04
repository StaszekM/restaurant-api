using RestaurantApi.Services;
using RestaurantApi.Entities;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Services.CreateScope().ServiceProvider.GetService<RestaurantSeeder>()?.Seed();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
