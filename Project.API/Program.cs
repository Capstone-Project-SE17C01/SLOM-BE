using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Project.API.Extensions;
using Project.API.Middlewares;
using Project.API.SignalR.Hubs;
using Project.API.SignalR.Service;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration
                .GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connection));

// Add services to the container.
builder.Services.AddCors();
builder.Services.RegisterService();
builder.Services.AddControllers();
builder.Services.AddControllers().AddOData(option => option.Select().Filter()
.Count().OrderBy().Expand().SetMaxTop(100));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<MessagingHub>("/hub");

app.UseCors(x => x
    .WithOrigins(builder.Configuration["CorsOrigin"])
        .AllowAnyHeader()
            .AllowAnyMethod()
                .AllowCredentials());

app.MapControllers();

app.UseRequestResponseLogging();

app.Run();
