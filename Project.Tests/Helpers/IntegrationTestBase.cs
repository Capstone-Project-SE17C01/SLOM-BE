using Microsoft.VisualStudio.TestPlatform.TestHost;
using Project.Infrastructure.Data;

namespace Project.Tests.Helpers;

[TestFixture]
public abstract class IntegrationTestBase
{
    protected TestWebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected ApplicationDbContext DbContext { get; private set; } = null!;

    [SetUp]
    public virtual void SetUp()
    {
        Factory = new TestWebApplicationFactory<Program>();
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is clean for each test
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

    [TearDown]
    public virtual void TearDown()
    {
        Client?.Dispose();
        Factory?.Dispose();
        DbContext?.Dispose();
    }

    protected async Task<T> GetFromJsonAsync<T>(string requestUri)
    {
        var response = await Client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content)!;
    }

    protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await Client.PostAsync(requestUri, content);
    }

    protected async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await Client.PutAsync(requestUri, content);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        return await Client.DeleteAsync(requestUri);
    }

    protected void SetAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    protected void ClearAuthorizationHeader()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}
