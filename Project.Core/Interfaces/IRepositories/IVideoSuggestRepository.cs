using Project.Core.Entities.Business.DTOs.VideoSuggestDTOs;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {

    public interface IVideoSuggestRepository : IBaseRepository<VideoSuggest> {
        public Task<VideoSuggestResponseDto> GetVideoSuggestsByUserId(VideoSuggestRequestDTO requestDTO);
    }
}
