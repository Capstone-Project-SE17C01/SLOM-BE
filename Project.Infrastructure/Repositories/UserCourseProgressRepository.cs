using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserCourseProgressRepository : BaseRepository<UserCourseProgress>, IUserCourseProgressRepository {
        public UserCourseProgressRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
        public async Task<int> CountCompletedAsync(Guid courseId, Guid userId) {
            return await _dbContext.UserCourseProgress
                .Where(x =>
                    x.CourseId == courseId &&
                    x.UserId == userId &&
                    x.CompletedAt != null
                )
                .CountAsync();
        }

        public async Task<int> CountLast7DaysCompletedCoursesAsync(Guid userId) {
            var last7Days = DateTime.UtcNow.AddDays(-7);

            return await _dbContext.UserCourseProgress
                .Where(x =>
                    x.CompletedAt != null &&
                    x.UserId == userId &&
                    x.CompletedAt >= last7Days
                )
                .CountAsync();
        }

        public async Task<List<Course>> GetCoursesByUserIdAsync(Guid userId) {
            return await _dbContext.UserCourseProgress
                .Include(x => x.Course)
                .Where(x => x.UserId == userId && x.Course != null)
                .Select(x => x.Course!)
                .ToListAsync();
        }

        public async Task MarkCourseCompleted(Guid userId, Guid courseId) {
            try {
                var userCourseProgress = await _dbContext.UserCourseProgress
                                .FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);

                if (userCourseProgress != null) {
                    userCourseProgress.CompletedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }
            } catch (Exception) {
                throw new Exception("Error marking course as completed");
            }
        }

        public async Task<bool> CheckIfAllModulesCompleted(Guid courseId, Guid userId) {
            var modules = await _dbContext.Modules
                .Where(x => x.CourseId == courseId)
                .ToListAsync();

            var completedModules = await _dbContext.UserModuleProgress
                .Where(x => x.UserId == userId && modules.Select(m => m.Id).Contains(x.ModuleId) && x.CompletedAt != null)
                .ToListAsync();

            return modules.Count == completedModules.Count;
        }
    }
}
