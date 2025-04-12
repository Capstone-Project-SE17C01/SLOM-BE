using System.ComponentModel.DataAnnotations;
using Net.payOS.Types;

namespace Project.Core.Entities.Business.DTOs.PaymentDTOs {

    namespace MyApi.Models {
        public class CreatePaymentRequest {
            public required Guid SubscriptionId { get; set; }

            public required Guid UserId { get; set; }

            public required string paymentMethod { get; set; }

            public required string Status { get; set; }

            public int durationMonth { get; set; }

            public string? productName { get; set; }

            public required string description { get; set; }

            public required string returnUrl { get; set; }

            public required string cancelUrl { get; set; }

            public int price { get; set; }

        }
    }

}
