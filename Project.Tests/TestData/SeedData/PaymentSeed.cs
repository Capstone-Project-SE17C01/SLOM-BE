using Project.Core.Entities.General;

namespace Project.Tests.TestData.SeedData
{
    public static class PaymentSeed
    {
        public static List<Payment> GetPayments()
        {
            return new List<Payment>
            {
                new Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    SubscriptionId = Guid.NewGuid(),
                    OrderCode = 1001,
                    Amount = 100000,
                    Currency = "VND",
                    PaymentMethod = "BankTransfer",
                    Status = "Success",
                    TransactionId = "TXN1001",
                    CreatedAt = DateTime.UtcNow
                },
                new Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    SubscriptionId = Guid.NewGuid(),
                    OrderCode = 1002,
                    Amount = 200000,
                    Currency = "VND",
                    PaymentMethod = "CreditCard",
                    Status = "Pending",
                    TransactionId = "TXN1002",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static Payment GetSinglePayment()
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                SubscriptionId = Guid.NewGuid(),
                OrderCode = 1099,
                Amount = 50000,
                Currency = "VND",
                PaymentMethod = "Test",
                Status = "Failed",
                TransactionId = "TXN1099",
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
