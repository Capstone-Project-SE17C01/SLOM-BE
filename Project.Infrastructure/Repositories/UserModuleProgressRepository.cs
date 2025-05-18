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
    }
}
