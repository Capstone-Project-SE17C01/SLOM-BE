using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IReminderRepository : IBaseRepository<Reminder> {
        Task<Reminder?> GetReminderByEmailAsync(string email, bool isActive);
    }
}
