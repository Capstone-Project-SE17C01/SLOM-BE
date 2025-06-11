namespace Project.Core.Entities.Business.DTOs.PaymentDTOs {
    public class HistoryPaymentDTO {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
