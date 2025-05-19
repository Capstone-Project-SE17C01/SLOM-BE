using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IUserLessonProgressRepository : IBaseRepository<UserLessonProgress> {
        Task<int> CountCompletedAsync(Guid moduleId, Guid userId);
        Task<int> CountLast7DaysCompletedLessonsAsync(Guid userId);
        Task<UserLessonProgress?> GetActiveLessonByUserIdAsync(Guid userId);
        public Task<List<UserLessonProgress>> GetLearnedLessons(Guid userId);
    }
}
