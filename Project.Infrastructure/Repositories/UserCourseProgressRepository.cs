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
    }
}
