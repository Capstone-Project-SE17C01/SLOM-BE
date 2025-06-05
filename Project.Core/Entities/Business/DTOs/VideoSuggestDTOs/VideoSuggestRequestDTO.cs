namespace Project.Core.Entities.Business.DTOs.VideoSuggestDTOs {
    public class VideoSuggestRequestDTO {
        public Guid UserId { get; set; }
        public string? SearchQuery { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
