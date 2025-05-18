namespace Project.Core.Entities.Business.DTOs.LanguageDTOs {
    public class ChangeLanguageRequestDTO {
        public required string email { get; set; }
        public required Guid languageId { get; set; }
        public required string newLanguageCode { get; set; }
    }
}
