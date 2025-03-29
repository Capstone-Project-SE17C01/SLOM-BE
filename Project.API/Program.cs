using Microsoft.EntityFrameworkCore;
using Project.API.Extensions;
using Project.API.Middlewares;
using Project.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(builder.Configuration
                .GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.RegisterService();
builder.Services.AddControllers();
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

app.UseRequestResponseLogging();

app.Run();
