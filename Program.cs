using RestaurantApi.Services;
using RestaurantApi.Entities;
using RestaurantApi.Middleware;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Console.WriteLine("Setting up services...");
builder.Services.AddControllers();
builder.Services.AddDbContext<RestaurantDbContext>();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<TimeTrackingMiddleware>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddSwaggerGen();

Console.WriteLine("Setting up logging...");
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

Console.WriteLine("Building...");
var app = builder.Build();

// Configure the HTTP request pipeline.

app.Services.CreateScope().ServiceProvider.GetService<RestaurantSeeder>()?.Seed();

app.UseMiddleware<TimeTrackingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RestaurantApi");
});

app.MapControllers();

Console.WriteLine("Running...");
app.Run();

NLog.LogManager.Shutdown();