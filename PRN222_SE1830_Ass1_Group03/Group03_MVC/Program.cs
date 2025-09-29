using BusinessObjects.DTO;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Service;
using DataAccessLayer.Repositories;

using BusinessObjects.Models; // ch?a Vehicle_Dealer_ManagementContext
using DataAccessLayer;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Services.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ??ng k� DbContext
builder.Services.AddDbContext<Vehicle_Dealer_ManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ??ng k� Repository + Service
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<VehicleRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<VehicleService>();

// Register DbContext first
builder.Services.AddDbContext<Vehicle_Dealer_ManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection"));
});

// Register DAOs and Repositories (these depend on DbContext)
builder.Services.AddScoped<AccountDao>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// Register Services (these depend on DAOs/Repositories)
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle Dealer Management API v1");
    c.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();