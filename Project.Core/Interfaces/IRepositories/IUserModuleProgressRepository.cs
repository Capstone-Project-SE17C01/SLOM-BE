using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IUserModuleProgressRepository : IBaseRepository<UserModuleProgress> {
        Task<int> CountCompletedAsync(Guid courseId, Guid userId);
        Task<int> CountLast7DaysCompletedModulesAsync(Guid userId);
        Task<bool> CheckIfAllLessonsCompleted(Guid moduleId, Guid userId);
        Task MarkModuleCompleted(Guid userId, Guid moduleId);
    }
}
