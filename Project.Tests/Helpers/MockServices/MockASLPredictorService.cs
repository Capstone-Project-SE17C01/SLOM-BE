namespace Project.Tests.Helpers.MockServices
{
    public class MockASLPredictorService
    {
        public Task<string> PredictAsync(byte[] imageData)
        {
            return Task.FromResult("MOCK_SIGN");
        }
    }
}
