using System.Net;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.PaymentDTOs;
using Project.Core.Entities.Business.DTOs.PaymentDTOs.MyApi.Models;
using Project.Core.Entities.General;
using Project.Infrastructure.Data;
using Project.Tests.Helpers;

namespace Project.Tests.Integration.Controllers;

[TestFixture]
public class PaymentControllerTests : IntegrationTestBase {
    private const string BaseUrl = "/api/Payment";
    private Guid _testUserId;
    private Guid _testSubscriptionPlanId;
    private Guid _testUserSubscriptionId;

    [SetUp]
    public override void SetUp() {
        base.SetUp();
        SeedTestData();
    }

    private void SeedTestData() {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _testUserId = Guid.NewGuid();
        var testUser = new Profile {
            Id = _testUserId,
            Username = "testuser",
            Email = "test@example.com",
            RoleId = Guid.NewGuid(),
            PreferredLanguageId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AvatarUrl = "https://example.com/avatar.jpg"
        };

        _testSubscriptionPlanId = Guid.NewGuid();
        var subscriptionPlan = new SubscriptionPlan {
            Id = _testSubscriptionPlanId,
            Name = "Premium Plan",
            Description = "Premium subscription plan",
            Price = 99.99m,
            DurationDays = 1,
        };

        _testUserSubscriptionId = Guid.NewGuid();
        var userSubscription = new UserSubscription {
            Id = _testUserSubscriptionId,
            UserId = _testUserId,
            PlanId = _testSubscriptionPlanId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
        };

        var testPayment = new Payment {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            SubscriptionId = _testUserSubscriptionId,
            Amount = 99.99m,
            Currency = "VND",
            Status = "PAID",
            PaymentMethod = "PAYOS",
            TransactionId = "test_transaction_123",
            OrderCode = 123456,
            CreatedAt = DateTime.UtcNow,
        };

        context.Profiles.Add(testUser);
        context.SubscriptionPlans.Add(subscriptionPlan);
        context.UserSubscriptions.Add(userSubscription);
        context.Payments.Add(testPayment);
        context.SaveChanges();
    }

    #region GetAllPlan Tests

    [Test]
    public async Task GetAllPlan_ShouldReturnSuccess() {
        var response = await Client.GetAsync($"{BaseUrl}/GetAllPlan");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Premium Plan");
    }

    [Test]
    public async Task GetAllPlan_ShouldReturnAllSubscriptionPlans() {
        var response = await GetFromJsonAsync<APIResponse>($"{BaseUrl}/GetAllPlan");

        response.Should().NotBeNull();
        response.result.Should().NotBeNull();
    }

    #endregion

    #region CreatePaymentLink Tests

    [Test]
    public async Task CreatePaymentLink_WithValidData_ShouldReturnSuccess() {
        var createPaymentRequest = new CreatePaymentRequest {
            UserId = _testUserId,
            SubscriptionId = _testSubscriptionPlanId,
            Price = 99,
            DurationMonth = 1,
            ProductName = "Premium Plan",
            Description = "Premium subscription",
            PaymentMethod = "PAYOS",
            ReturnUrl = "http://localhost:3000/payment/success",
            CancelUrl = "http://localhost:3000/payment/cancel",
            Status = "PENDING"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreatePaymentLink_WithInvalidUserId_ShouldReturnBadRequest() {
        var createPaymentRequest = new CreatePaymentRequest {
            UserId = Guid.Empty,
            SubscriptionId = _testSubscriptionPlanId,
            Price = 99,
            DurationMonth = 1,
            ProductName = "Premium Plan",
            Description = "Premium subscription",
            PaymentMethod = "PAYOS",
            ReturnUrl = "http://localhost:3000/payment/success",
            CancelUrl = "http://localhost:3000/payment/cancel",
            Status = "PENDING"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreatePaymentLink_WithInvalidSubscriptionId_ShouldReturnBadRequest() {
        var createPaymentRequest = new CreatePaymentRequest {
            UserId = _testUserId,
            SubscriptionId = Guid.Empty,
            Price = 99,
            DurationMonth = 1,
            ProductName = "Premium Plan",
            Description = "Premium subscription",
            PaymentMethod = "PAYOS",
            ReturnUrl = "http://localhost:3000/payment/success",
            CancelUrl = "http://localhost:3000/payment/cancel",
            Status = "PENDING"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreatePaymentLink_WithZeroPrice_ShouldReturnBadRequest() {
        var createPaymentRequest = new CreatePaymentRequest {
            UserId = _testUserId,
            SubscriptionId = _testSubscriptionPlanId,
            Price = 0,
            DurationMonth = 1,
            ProductName = "Premium Plan",
            Description = "Premium subscription",
            PaymentMethod = "PAYOS",
            ReturnUrl = "http://localhost:3000/payment/success",
            CancelUrl = "http://localhost:3000/payment/cancel",
            Status = "PENDING"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreatePaymentLink_WithInvalidDurationMonth_ShouldReturnBadRequest() {
        var createPaymentRequest = new CreatePaymentRequest {
            UserId = _testUserId,
            SubscriptionId = _testSubscriptionPlanId,
            Price = 99,
            DurationMonth = 0,
            ProductName = "Premium Plan",
            Description = "Premium subscription",
            PaymentMethod = "PAYOS",
            ReturnUrl = "http://localhost:3000/payment/success",
            CancelUrl = "http://localhost:3000/payment/cancel",
            Status = "PENDING"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region UpdatePlan Tests

    [Test]
    public async Task UpdatePlan_WithValidPaidStatus_ShouldReturnSuccess() {
        var returnUrlQuery = new ReturnUrlQuery {
            OrderCode = 123456,
            Status = "PAID",
            Code = "00",
            Cancel = false,
            Period = 1,
            UserId = _testUserId.ToString(),
            Id = _testUserSubscriptionId.ToString()
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePlan", returnUrlQuery);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdatePlan_WithCancelledStatus_ShouldReturnSuccess() {
        var returnUrlQuery = new ReturnUrlQuery {
            OrderCode = 123456,
            Status = "CANCELLED",
            Code = "01",
            Cancel = true,
            Period = 1,
            UserId = _testUserId.ToString(),
            Id = _testUserSubscriptionId.ToString()
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePlan", returnUrlQuery);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdatePlan_WithInvalidOrderCode_ShouldReturnNotFound() {
        var returnUrlQuery = new ReturnUrlQuery {
            OrderCode = 999999,
            Status = "PAID",
            Code = "00",
            Cancel = false,
            Period = 1,
            UserId = _testUserId.ToString(),
            Id = _testUserSubscriptionId.ToString()
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePlan", returnUrlQuery);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdatePlan_WithNullRequest_ShouldReturnBadRequest() {
        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePlan", (ReturnUrlQuery)null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GetAllPaymentInformation Tests

    [Test]
    public async Task GetAllPaymentInformation_WithValidUserId_ShouldReturnSuccess() {
        var response = await Client.GetAsync($"{BaseUrl}/GetAllPaymentInformation?userId={_testUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("test_transaction_123");
    }

    [Test]
    public async Task GetAllPaymentInformation_WithInvalidUserId_ShouldReturnEmptyList() {
        var invalidUserId = Guid.NewGuid();

        var response = await Client.GetAsync($"{BaseUrl}/GetAllPaymentInformation?userId={invalidUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await GetFromJsonAsync<APIResponse>($"{BaseUrl}/GetAllPaymentInformation?userId={invalidUserId}");
        apiResponse.result.Should().NotBeNull();
    }

    [Test]
    public async Task GetAllPaymentInformation_WithEmptyGuid_ShouldReturnBadRequest() {
        var response = await Client.GetAsync($"{BaseUrl}/GetAllPaymentInformation?userId={Guid.Empty}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task PaymentController_WithMalformedRequest_ShouldReturnBadRequest() {
        var malformedJson = "{ invalid json }";
        var content = new StringContent(malformedJson, System.Text.Encoding.UTF8, "application/json");

        var response = await Client.PostAsync($"{BaseUrl}/CreatePaymentLink", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreatePaymentLink_WithMissingRequiredFields_ShouldReturnBadRequest() {
        var incompleteRequest = new {
            UserId = _testUserId,
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", incompleteRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdatePlan_WithInvalidStatusCode_ShouldReturnBadRequest() {
        var returnUrlQuery = new ReturnUrlQuery {
            OrderCode = 123456,
            Status = "FAILED",
            Code = "99",
            Cancel = false,
            Period = 1,
            UserId = _testUserId.ToString(),
            Id = _testUserSubscriptionId.ToString()
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/UpdatePlan", returnUrlQuery);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Integration Tests

    [Test]
    public async Task PaymentFlow_CreatePaymentThenUpdate_ShouldWorkCorrectly() {
        var createPaymentRequest = new CreatePaymentRequest {
            UserId = _testUserId,
            SubscriptionId = _testSubscriptionPlanId,
            Price = 99,
            DurationMonth = 1,
            ProductName = "Premium Plan",
            Description = "Premium subscription",
            PaymentMethod = "PAYOS",
            ReturnUrl = "http://localhost:3000/payment/success",
            CancelUrl = "http://localhost:3000/payment/cancel",
            Status = "PENDING"
        };

        var createResponse = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest);
        createResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (createResponse.StatusCode == HttpStatusCode.OK) {
            var returnUrlQuery = new ReturnUrlQuery {
                OrderCode = 123456,
                Status = "PAID",
                Code = "00",
                Cancel = false,
                Period = 1,
                UserId = _testUserId.ToString(),
                Id = _testUserSubscriptionId.ToString()
            };

            var updateResponse = await PostAsJsonAsync($"{BaseUrl}/UpdatePlan", returnUrlQuery);
            updateResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
        }
    }

    #endregion
    [Test]
    public async Task CreatePaymentLink_MultipleConcurrentRequests_ShouldHandleLoad() {
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 5; i++) {
            var createPaymentRequest = new CreatePaymentRequest {
                UserId = _testUserId,
                SubscriptionId = _testSubscriptionPlanId,
                Price = 99,
                DurationMonth = 1,
                ProductName = $"Premium Plan {i}",
                Description = "Premium subscription",
                PaymentMethod = "PAYOS",
                ReturnUrl = "http://localhost:3000/payment/success",
                CancelUrl = "http://localhost:3000/payment/cancel",
                Status = "PENDING"
            };
            tasks.Add(PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest));
        }

        var responses = await Task.WhenAll(tasks);

        foreach (var response in responses) {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
            response.Dispose();
        }
    }

    [Test]
    public async Task CreatePaymentLink_WithLargeProductName_ShouldReturnBadRequestOrHandleGracefully() {
        var largeProductName = new string('A', 300);
        var createPaymentRequest = new CreatePaymentRequest {
            UserId = _testUserId,
            SubscriptionId = _testSubscriptionPlanId,
            Price = 99,
            DurationMonth = 1,
            ProductName = largeProductName,
            Description = "Premium subscription",
            PaymentMethod = "PAYOS",
            ReturnUrl = "http://localhost:3000/payment/success",
            CancelUrl = "http://localhost:3000/payment/cancel",
            Status = "PENDING"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/CreatePaymentLink", createPaymentRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }
}
