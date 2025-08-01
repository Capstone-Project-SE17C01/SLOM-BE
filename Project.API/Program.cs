using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.API.Extensions;
using Project.API.Middlewares;
using Project.API.SignalR.Hubs;
using Project.Core.Exceptions;
using Project.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connection = Environment.GetEnvironmentVariable("DefaultConnection") ?? builder.Configuration.GetConnectionString("DefaultConnection")
                                                                         ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connection));

// Add services to the container.
builder.Services.AddCors();
builder.Services.RegisterService(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(opts => {
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opts.JsonSerializerOptions.MaxDepth = 64;
});
builder.Services.AddControllers().AddOData(option => option.Select().Filter()
.Count().OrderBy().Expand().SetMaxTop(100));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "SLOM API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.AddSignalR();
builder.Services.AddHostedService<ReminderMeeting>();
builder.Services.AddHostedService<ReminderNotifier>();

string region = builder.Configuration["AWS:Region"] ?? throw new NotFoundException();
string userPoolId = builder.Configuration["AWS:UserPoolId"] ?? throw new NotFoundException();
string clientId = builder.Configuration["AWS:ClientId"] ?? throw new NotFoundException();

string cognitoOpenIdConfigUrl = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}/.well-known/openid-configuration";
string cognitoIssuer = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        cognitoOpenIdConfigUrl,
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever());

    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => {
            var configuration = configurationManager.GetConfigurationAsync(CancellationToken.None).GetAwaiter().GetResult();
            return configuration.SigningKeys;
        },
        ValidateIssuer = true,
        ValidIssuer = cognitoIssuer,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MessagingHub>("/hub");

app.UseCors(x => {
    x.WithOrigins(builder.Configuration["CorsOrigin"] ?? "")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

app.MapControllers();

app.UseRequestResponseLogging();

app.Run();
