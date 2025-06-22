using System.Text.RegularExpressions;
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

        public async Task<VideoSuggestResponseDto> GetVideoSuggestsByUserId(VideoSuggestRequestDTO requestDTO) {
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

            var videoSuggestQuery = _dbContext.VideoSuggests;

            var listVideoSuggest = new List<ListVideoSuggest>();

            VideoSuggestResponseDto result = new VideoSuggestResponseDto();

            if (!learnedKeywords.Any() && !searchKeywords.Any()) {
                listVideoSuggest = await videoSuggestQuery
                    .OrderByDescending(v => v.PublishDate)
                    .Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize)
                    .Take(requestDTO.PageSize)
                    .Select(video => new ListVideoSuggest {
                        Id = video.Id,
                        Title = video.Title,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl,
                        VideoThumbnail = GetYouTubeThumbnail(video.VideoUrl),
                        VideoId = GetVideoId(video.VideoUrl)
                    })
                    .AsNoTracking()
                    .ToListAsync();

                result = new VideoSuggestResponseDto() {
                    isLoadFullPage = videoSuggestQuery.Count() <= requestDTO.PageSize * requestDTO.PageNumber,
                    VideoSuggest = listVideoSuggest,
                };

                return result;
            }

            var allKeywords = learnedKeywords
                .Concat(searchKeywords)
                .Distinct()
                .ToList();

            var keywordPatterns = allKeywords
                .Select(kw => $"%{kw}%")
                .ToList();

            var query = videoSuggestQuery
                .Where(video =>
                    video.Title != null &&
                    keywordPatterns.Any(pattern =>
                        EF.Functions.ILike(video.Title, pattern)
                    )
                )
                .OrderBy(v => v.Title)
                .Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize)
                .Take(requestDTO.PageSize)
                .Select(video => new ListVideoSuggest {
                    Id = video.Id,
                    Title = video.Title,
                    Description = video.Description,
                    VideoUrl = video.VideoUrl,
                    VideoThumbnail = GetYouTubeThumbnail(video.VideoUrl),
                    VideoId = GetVideoId(video.VideoUrl)
                })
                .AsNoTracking();

            result = new VideoSuggestResponseDto() {
                isLoadFullPage = videoSuggestQuery.Count() <= requestDTO.PageSize * requestDTO.PageNumber,
                VideoSuggest = await query.ToListAsync(),
            };

            return result;
        }

        private static string? GetYouTubeThumbnail(string? videoUrl) {
            if (videoUrl != null) {
                string pattern = @"(?:https?:\/\/(?:www\.)?youtube\.com\/watch\?v=([\w\-]+))";
                Regex regex = new Regex(pattern);

                var match = regex.Match(videoUrl);

                if (match.Success) {
                    string videoId = match.Groups[1].Value;

                    if (!string.IsNullOrEmpty(videoId)) {
                        return $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
                    }
                }
            }

            return null;
        }

        private static string? GetVideoId(string? youtubeUrl) {
            if (youtubeUrl != null) {
                string pattern = @"(?:https?:\/\/(?:www\.)?youtube\.com\/watch\?v=([\w\-]+))";
                Regex regex = new Regex(pattern);

                var match = regex.Match(youtubeUrl);

                if (match.Success) {
                    string videoId = match.Groups[1].Value;

                    if (!string.IsNullOrEmpty(videoId)) {
                        return videoId;
                    }
                }
            }

            return null;
        }
    }
}
