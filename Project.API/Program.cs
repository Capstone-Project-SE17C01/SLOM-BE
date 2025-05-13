using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Project.API.Extensions;
using Project.API.Middlewares;
using Project.API.SignalR.Hubs;
using Project.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = builder.Configuration["KeyVault:KeyVaultURL"];
if (!string.IsNullOrEmpty(keyVaultEndpoint)) {
    try {
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions {
            ExcludeEnvironmentCredential = false,
            ExcludeAzureCliCredential = false,
            ExcludeManagedIdentityCredential = false,
            ExcludeSharedTokenCacheCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = true,
            ExcludeInteractiveBrowserCredential = true
        });

        var client = new SecretClient(new Uri(keyVaultEndpoint), credential);
        builder.Configuration.AddAzureKeyVault(client, new KeyVaultSecretManager());
    }
    catch (Exception ex) {
        Console.WriteLine($"Error configuring Key Vault: {ex.Message}");
    }
}
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
