using APIRestCOURS.DataAccess;
using APIRestCOURS.DataAccess.Interfaces;
using APIRestCOURS.Middleware;
using APIRestCOURS.Service;
using APIRestCOURS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configure SQLite with EF Core
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Data Source=bank.db"));

// Register Data Access Layer
builder.Services.AddScoped<IBankDataAccess, BankDataAccess>();

// Register Service Layer
builder.Services.AddScoped<IBankService, BankService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Bank API",
        Version = "v1",
        Description = "API REST pour la gestion d'une banque avec EF Core - B3 Dev"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank API v1");
    });
}

app.UseAuditMiddleware();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
