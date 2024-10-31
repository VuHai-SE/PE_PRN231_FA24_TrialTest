using Domain.Interfaces;
using Domain.Interfaces.Services;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ViroCureFal2024dbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")).EnableSensitiveDataLogging();
});

// Add DbFactory and UnitOfWork
builder.Services.AddScoped<Func<ViroCureFal2024dbContext>>((provider) => () => provider.GetService<ViroCureFal2024dbContext>());
builder.Services.AddScoped<DbFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IViroCureUserService, ViroCureUserService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
