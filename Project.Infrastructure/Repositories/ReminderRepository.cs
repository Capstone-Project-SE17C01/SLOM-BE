using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class ReminderRepository : BaseRepository<Reminder>, IReminderRepository {
        public ReminderRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<Reminder?> GetReminderByEmailAsync(string email, bool isActive) {
            return await _dbContext.Reminders
                .FirstOrDefaultAsync(r => r.Email == email && (!isActive || r.IsActive));
        }

    }
}
