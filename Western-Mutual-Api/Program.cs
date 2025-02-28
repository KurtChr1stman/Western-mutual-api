using Microsoft.EntityFrameworkCore;
using Western_Mutual_Api.Data;
using Western_Mutual_Api.Interfaces;
using Western_Mutual_Api.Interfaces.Dapper;
using Western_Mutual_Api.Models;
using Western_Mutual_Api.Repository;
using Western_Mutual_Api.Services;
using Western_Mutual_Api.Services.Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductService, ProductService>(); 
builder.Services.AddScoped<IBuyerService, BuyerService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IProductDapperService, ProductDapperService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
