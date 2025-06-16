using Project.Core.Entities.General;

namespace Project.Core.Entities.Business.DTOs.VideoSuggestDTOs {
    public class VideoSuggestResponseDto {
        public List<ListVideoSuggest>? VideoSuggest { get; set; }
        public bool isLoadFullPage { get; set; }
    }

    public class ListVideoSuggest : VideoSuggest {
        public string? VideoThumbnail { get; set; }
        public string? VideoId { get; set; }
    }
}
