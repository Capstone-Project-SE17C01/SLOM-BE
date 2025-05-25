namespace Project.Core.Entities.Business.DTOs.PaymentDTOs {
    public class ReturnUrlQuery {
        public required string UserId { get; set; }

        public required string Code { get; set; }

        public required string Id { get; set; }

        public bool Cancel { get; set; }

        public required string Status { get; set; }

        public int OrderCode { get; set; }

        public int Period { get; set; }
    }

}
