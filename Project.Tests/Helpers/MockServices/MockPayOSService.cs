namespace Project.Tests.Helpers.MockServices {
    public class MockPayOSService {
        public Task<string> CreatePaymentLinkAsync(decimal amount, string description) {
            return Task.FromResult("https://mock.payos.vn/payment-link");
        }

        public Task<string> GetPaymentStatusAsync(string paymentId) {
            return Task.FromResult("SUCCESS");
        }
    }
}
