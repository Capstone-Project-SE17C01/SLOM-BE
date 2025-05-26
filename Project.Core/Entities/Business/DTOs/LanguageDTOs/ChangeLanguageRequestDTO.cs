namespace Project.Core.Entities.Business.DTOs.LanguageDTOs {
    public class ChangeLanguageRequestDTO {
        public required string Email { get; set; }
        public required Guid LanguageId { get; set; }
        public required string NewLanguageCode { get; set; }
    }
}
