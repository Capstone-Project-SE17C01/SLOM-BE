namespace Project.Core.Entities.Business.DTOs.LanguageDTOs {
    public class ChangeLanguageResponseDTO {
        public required Guid LanguageId { get; set; }
        public required string LanguageCode { get; set; }
    }
}
