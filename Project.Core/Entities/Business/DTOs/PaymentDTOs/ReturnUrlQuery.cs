using System.ComponentModel.DataAnnotations;

namespace Project.Core.Entities.Business.DTOs.PaymentDTOs {
    public class ReturnUrlQuery {
        public required string userId { get; set; }

        public required string code { get; set; }

        public required string id { get; set; }

        public bool cancel { get; set; }

        [RegularExpression("^(PAID|PENDING|PROCESSING|CANCELLED)$",
        ErrorMessage = "status must be: PAID, PENDING, PROCESSING, CANCELLED")]
        public required string status { get; set; }

        public int orderCode { get; set; }

        public int period { get; set; }
    }

}
