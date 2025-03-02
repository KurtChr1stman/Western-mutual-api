using Microsoft.EntityFrameworkCore;
using System.Text;
using Western_Mutual_Api.Data;
using Western_Mutual_Api.Interfaces;
using Western_Mutual_Api.Interfaces.Dapper;
using Western_Mutual_Api.Models;
using Western_Mutual_Api.Repository;
using Western_Mutual_Api.Services;
using Western_Mutual_Api.Services.Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]);
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<JwtService>(); // Register JWT Service
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication(); // Enable authentication

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
