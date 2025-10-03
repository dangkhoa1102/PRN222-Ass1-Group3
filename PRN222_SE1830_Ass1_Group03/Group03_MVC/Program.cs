using BusinessObjects.Models;              // giữ DbContext chuẩn
using DataAccessLayer;
using DataAccessLayer.Repositories;        // repo ở đây
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Đăng ký DbContext (chỉ lấy từ BusinessObjects.Models)
builder.Services.AddDbContext<Vehicle_Dealer_ManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection")));

// Register DAOs and Repositories (these depend on DbContext)
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDealerRepository, DealerRepository>();
builder.Services.AddScoped<ITestDriveApoitmentRepository, StaffTestDriveAppoitmentRepository>();
//builder.Services.AddScoped<AccountDao>();

// Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDealerService, DealerService>();
builder.Services.AddScoped<ITestDriveAppointmentService, StaffTestDriveAppointmentService>();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
