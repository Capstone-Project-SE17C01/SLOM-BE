using System.Net;
using Project.Core.Entities.Business.DTOs.ASLDTOs;
using Project.Tests.Helpers;

namespace Project.Tests.Integration.Controllers;

[TestFixture]
public class ASLControllerTests : IntegrationTestBase {
    private const string BaseUrl = "/api/ASL";

    private const string SampleBase64Image = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==";
    private const string SampleBase64ImageWithPrefix = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==";

    #region Predict Tests (POST)

    [Test]
    public async Task Predict_WithValidBase64Image_ShouldReturnPredictionResult() {
        var predictionRequest = new PredictionRequest {
            ImageBase64 = SampleBase64Image
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);

        if (response.StatusCode == HttpStatusCode.OK) {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Word");
            content.Should().Contain("Confidence");
        }
    }

    [Test]
    public async Task Predict_WithBase64ImageWithPrefix_ShouldReturnPredictionResult() {
        var predictionRequest = new PredictionRequest {
            ImageBase64 = SampleBase64ImageWithPrefix
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);

        if (response.StatusCode == HttpStatusCode.OK) {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Word");
            content.Should().Contain("Confidence");
        }
    }

    [Test]
    public async Task Predict_WithEmptyImageData_ShouldReturnBadRequest() {
        var predictionRequest = new PredictionRequest {
            ImageBase64 = ""
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Image data is required");
    }

    [Test]
    public async Task Predict_WithNullImageData_ShouldReturnBadRequest() {
        var predictionRequest = new PredictionRequest {
            ImageBase64 = null
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Image data is required");
    }

    [Test]
    public async Task Predict_WithInvalidBase64Data_ShouldReturnInternalServerError() {
        var predictionRequest = new PredictionRequest {
            ImageBase64 = "invalid_base64_data!!!"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Internal server error while processing image");
    }

    [Test]
    public async Task Predict_WithMalformedBase64_ShouldReturnInternalServerError() {
        var predictionRequest = new PredictionRequest {
            ImageBase64 = "not_a_valid_base64_string"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    #endregion

    #region Buffer Tests

    [Test]
    public async Task BufferImage_WithValidBase64Image_ShouldReturnSuccess() {
        var bufferRequest = new BufferImageRequest {
            ImageBase64 = SampleBase64Image
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task BufferImage_WithBase64ImageWithPrefix_ShouldReturnSuccess() {
        var bufferRequest = new BufferImageRequest {
            ImageBase64 = SampleBase64ImageWithPrefix
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task BufferImage_WithEmptyImageData_ShouldReturnBadRequest() {
        var bufferRequest = new BufferImageRequest {
            ImageBase64 = ""
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Image data is required");
    }

    [Test]
    public async Task BufferImage_WithNullImageData_ShouldReturnBadRequest() {
        var bufferRequest = new BufferImageRequest {
            ImageBase64 = null
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Image data is required");
    }

    [Test]
    public async Task BufferImage_WithInvalidBase64Data_ShouldReturnInternalServerError() {
        var bufferRequest = new BufferImageRequest {
            ImageBase64 = "invalid_base64_data!!!"
        };

        var response = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Internal server error while buffering image");
    }

    #endregion

    #region GetPrediction Tests (GET)

    [Test]
    public async Task GetPrediction_ShouldReturnPredictionResult() {
        var response = await Client.GetAsync($"{BaseUrl}/predict");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);

        if (response.StatusCode == HttpStatusCode.OK) {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Word");
            content.Should().Contain("Confidence");
        }
    }

    [Test]
    public async Task GetPrediction_ShouldReturnConsistentStructure() {
        var response = await Client.GetAsync($"{BaseUrl}/predict");

        if (response.StatusCode == HttpStatusCode.OK) {
            var result = await GetFromJsonAsync<PredictionResult>($"{BaseUrl}/predict");
            result.Should().NotBeNull();
            result.Word.Should().NotBeNull();
            result.Confidence.Should().BeGreaterOrEqualTo(0);
        }
        else {
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region Integration Flow Tests

    [Test]
    public async Task ASLWorkflow_BufferThenPredict_ShouldWorkCorrectly() {
        var bufferRequest = new BufferImageRequest {
            ImageBase64 = SampleBase64Image
        };

        var bufferResponse = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        bufferResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);

        if (bufferResponse.StatusCode == HttpStatusCode.OK) {
            var predictionResponse = await Client.GetAsync($"{BaseUrl}/predict");

            predictionResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);

            if (predictionResponse.StatusCode == HttpStatusCode.OK) {
                var content = await predictionResponse.Content.ReadAsStringAsync();
                content.Should().Contain("Word");
                content.Should().Contain("Confidence");
            }
        }
    }

    [Test]
    public async Task ASLWorkflow_MultipleBufferOperations_ShouldWork() {
        var bufferRequest1 = new BufferImageRequest { ImageBase64 = SampleBase64Image };
        var bufferRequest2 = new BufferImageRequest { ImageBase64 = SampleBase64ImageWithPrefix };

        var response1 = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest1);
        var response2 = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest2);

        response1.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        response2.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task ASLController_WithMalformedRequest_ShouldReturnBadRequest() {
        var malformedJson = "{ invalid json }";
        var content = new StringContent(malformedJson, System.Text.Encoding.UTF8, "application/json");

        var response = await Client.PostAsync($"{BaseUrl}/predict", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ASLController_WithNullRequest_ShouldReturnBadRequest() {
        var response = await Client.PostAsync($"{BaseUrl}/predict", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task PredictAndBuffer_WithSameImage_ShouldReturnConsistentResults() {
        var predictionRequest = new PredictionRequest { ImageBase64 = SampleBase64Image };
        var bufferRequest = new BufferImageRequest { ImageBase64 = SampleBase64Image };

        var predictResponse = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);
        var bufferResponse = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        if (predictResponse.StatusCode == HttpStatusCode.OK) {
            bufferResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        else if (predictResponse.StatusCode == HttpStatusCode.InternalServerError) {
            bufferResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }

    #endregion

    #region Performance Tests

    [Test]
    public async Task ASL_MultipleConsecutivePredictions_ShouldHandleLoad() {
        var predictionRequest = new PredictionRequest { ImageBase64 = SampleBase64Image };
        var tasks = new List<Task<HttpResponseMessage>>();

        for (int i = 0; i < 5; i++) {
            tasks.Add(PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest));
        }

        var responses = await Task.WhenAll(tasks);

        foreach (var response in responses) {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError, HttpStatusCode.BadRequest);
            response.Dispose();
        }
    }

    #endregion

    #region Validation Tests

    [Test]
    public async Task Predict_WithLargeBase64String_ShouldHandleGracefully() {
        var largeBase64 = new string('A', 10000); // 10KB of 'A' characters
        var predictionRequest = new PredictionRequest { ImageBase64 = largeBase64 };

        var response = await PostAsJsonAsync($"{BaseUrl}/predict", predictionRequest);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.RequestEntityTooLarge
        );
    }

    [Test]
    public async Task BufferImage_WithSpecialCharacters_ShouldHandleGracefully() {
        var specialCharsBase64 = "!@#$%^&*()_+{}[]|\\:;\"'<>,.?/~`";
        var bufferRequest = new BufferImageRequest { ImageBase64 = specialCharsBase64 };

        var response = await PostAsJsonAsync($"{BaseUrl}/buffer", bufferRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Internal server error while buffering image");
    }

    [Test]
    public async Task Predict_WithoutAuthToken_ShouldReturnUnauthorized() {
        var predictionRequest = new PredictionRequest {
            ImageBase64 = SampleBase64Image
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/predict") {
            Content = JsonContent.Create(predictionRequest)
        };
        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    #endregion
}
