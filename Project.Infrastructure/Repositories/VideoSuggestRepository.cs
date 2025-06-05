using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Business.DTOs.VideoSuggestDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class VideoSuggestRepository : BaseRepository<VideoSuggest>, IVideoSuggestRepository {
        private readonly IUserLessonProgressRepository _userLessonRepo;
        public VideoSuggestRepository(ApplicationDbContext dbContext) : base(dbContext) {
            _userLessonRepo = new UserLessonProgressRepository(dbContext);
        }
        public async Task<List<VideoSuggest>> GetVideoSuggestsByUserId(VideoSuggestRequestDTO requestDTO) {
            var learnedTitles = await _userLessonRepo.GetTitleLearnedLessons(requestDTO.UserId);

            var learnedKeywords = learnedTitles
                .Where(title => !string.IsNullOrWhiteSpace(title))
                .SelectMany(title => title.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(word => word.ToLowerInvariant())
                .Distinct()
                .ToList();

            var searchKeywords = string.IsNullOrWhiteSpace(requestDTO.SearchQuery)
                ? new List<string>()
                : requestDTO.SearchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.ToLowerInvariant())
                    .Distinct()
                    .ToList();

            if (!learnedKeywords.Any() && !searchKeywords.Any()) {
                return await _dbContext.VideoSuggests
                    .OrderByDescending(v => v.PublishDate)
                    .Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize)
                    .Take(requestDTO.PageSize)
                    .Select(video => new VideoSuggest {
                        Id = video.Id,
                        Title = video.Title,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl
                    })
                    .AsNoTracking()
                    .ToListAsync();
            }

            var allKeywords = learnedKeywords
                .Concat(searchKeywords)
                .Distinct()
                .ToList();

            var keywordPatterns = allKeywords
                .Select(kw => $"%{kw}%")
                .ToList();

            var query = _dbContext.VideoSuggests
                .Where(video =>
                    video.Title != null &&
                    keywordPatterns.Any(pattern =>
                        EF.Functions.ILike(video.Title, pattern)
                    )
                )
                .OrderBy(v => v.Title)
                .Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize)
                .Take(requestDTO.PageSize)
                .Select(video => new VideoSuggest {
                    Id = video.Id,
                    Title = video.Title,
                    Description = video.Description,
                    VideoUrl = video.VideoUrl
                })
                .AsNoTracking();
            return await query.ToListAsync();
        }
    }
}
