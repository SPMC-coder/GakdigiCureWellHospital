using Microsoft.EntityFrameworkCore;
using CureWellHospital.Data;
using CureWellHospital.Interfaces;
using CureWellHospital;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register EF Core DbContext with SQL Server
builder.Services.AddDbContext<CureWellHospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CureWellHospitalDB")));

// Register repository for dependency injection
builder.Services.AddScoped<ICureWellHospitalRepository, CureWellHospitalRepository>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API controllers (needed for [ApiController] attribute routing)
app.MapControllers();

app.Run();
