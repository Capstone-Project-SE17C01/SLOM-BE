using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserLessonProgressRepository : BaseRepository<UserLessonProgress>, IUserLessonProgressRepository {
        public UserLessonProgressRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<int> CountCompletedAsync(Guid courseId, Guid userId) {
            List<UserModuleProgress> userModuleProgresses = await _dbContext.UserModuleProgress
                .Include(x => x.Module)
                .Where(x =>
                    x.Module != null &&
                    x.Module.CourseId == courseId &&
                    x.UserId == userId
                )
                .ToListAsync();
            List<UserLessonProgress> userLessonProgresses = await _dbContext.UserLessonProgress
                .Include(x => x.Lesson)
                .Where(x =>
                    x.Lesson != null &&
                    userModuleProgresses.Select(m => m.ModuleId).Contains(x.Lesson.ModuleId) &&
                    x.UserId == userId
                )
                .ToListAsync();

            return userLessonProgresses
                .Where(x => x.CompletedAt != null)
                .Count();
        }

        public async Task<int> CountLast7DaysCompletedLessonsAsync(Guid userId) {
            var last7Days = DateTime.UtcNow.AddDays(-7);

            return await _dbContext.UserLessonProgress
                .Where(x =>
                    x.CompletedAt != null &&
                    x.UserId == userId &&
                    x.CompletedAt >= last7Days
                )
                .CountAsync();
        }

        public async Task<UserLessonProgress?> GetActiveLessonByUserIdAsync(Guid userId) {
            return await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId && x.IsActive)
                .Include(x => x.Lesson)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserLessonProgress>> GetLearnedLessons(Guid userId) {
            var result = await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId)
                .Include(x => x.Lesson)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

        public async Task<bool> CreateNewLessonProgress(Guid userId, Guid lessonId) {
            var user = _dbContext.Profiles.Where(x => x.Id == userId)
                .FirstOrDefault();
            var lesson = _dbContext.Lessons.Where(x => x.Id == lessonId)
                .FirstOrDefault();

            if (user is null || lesson is null) {
                throw new Exception(user is null ? "User not found" : "Lesson not found");
            }

            if (_dbContext.UserLessonProgress.Where(x => x.UserId == userId && x.LessonId == lessonId).AsNoTracking().Count() > 0) {
                throw new Exception("Progress created");
            }

            UserLessonProgress progress = new UserLessonProgress() {
                LessonId = lessonId,
                UserId = userId,
                IsActive = true,
                User = user,
                Lesson = lesson,
            };

            try {
                _dbContext.UserLessonProgress.Add(progress);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch {
                return false;
            }
        }

        public async Task<bool> CompleteLessons(Guid userId, Guid lessonId) {
            var progress = await _dbContext.UserLessonProgress
                .Where(x => x.UserId == userId && x.LessonId == lessonId)
                .FirstOrDefaultAsync();

            if (progress is null) {
                throw new Exception("You have not learned this lessoon");
            }

            progress.CompletedAt = DateTime.UtcNow;
            try {
                _dbContext.UserLessonProgress.Update(progress);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
