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
                .FirstOrDefaultAsync();
        }
    }
}
