namespace Project.Core.Entities.Business.DTOs.PaymentDTOs {

    namespace MyApi.Models {
        public class CreatePaymentRequest {
            public required Guid SubscriptionId { get; set; }

            public required Guid UserId { get; set; }

            public required string PaymentMethod { get; set; }

            public required string Status { get; set; }

            public int DurationMonth { get; set; }

            public string? ProductName { get; set; }

            public required string Description { get; set; }

            public required string ReturnUrl { get; set; }

            public required string CancelUrl { get; set; }

            public int Price { get; set; }

        }
    }

}
