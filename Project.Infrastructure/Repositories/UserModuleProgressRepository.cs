using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserModuleProgressRepository : BaseRepository<UserModuleProgress>, IUserModuleProgressRepository {
        public UserModuleProgressRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<int> CountCompletedAsync(Guid courseId, Guid userId) {
            return await _dbContext.UserModuleProgress
                .Include(x => x.Module)
                .Where(x =>
                    x.Module != null &&
                    x.Module.CourseId == courseId &&
                    x.UserId == userId &&
                    x.CompletedAt != null
                )
                .CountAsync();
        }

        public async Task<int> CountLast7DaysCompletedModulesAsync(Guid userId) {
            var last7Days = DateTime.UtcNow.AddDays(-7);

            return await _dbContext.UserModuleProgress
                .Where(x =>
                    x.CompletedAt != null &&
                    x.UserId == userId &&
                    x.CompletedAt >= last7Days
                )
                .CountAsync();
        }

        public async Task MarkModuleCompleted(Guid userId, Guid moduleId) {
            try {
                var userModuleProgress = await _dbContext.UserModuleProgress
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ModuleId == moduleId);

                if (userModuleProgress != null) {
                    userModuleProgress.CompletedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }
            } catch (Exception) {
                throw new Exception("Error marking module as completed");
            }
        }
        public async Task<bool> CheckIfAllLessonsCompleted(Guid moduleId, Guid userId) {
            try {
                var lessons = await _dbContext.Lessons
                    .Where(x => x.ModuleId == moduleId)
                    .ToListAsync();

                var lessonIds = lessons.Select(l => l.Id).ToList();

                var completedLessons = await _dbContext.UserLessonProgress
                    .Where(x => x.UserId == userId && lessonIds.Contains(x.LessonId) && x.CompletedAt != null)
                    .ToListAsync();

                return lessons.Count == completedLessons.Count;
            } catch (Exception) {
                return false;
            }
        }
    }
}
