using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IUserLessonProgressRepository : IBaseRepository<UserLessonProgress> {
        Task<int> CountCompletedAsync(Guid moduleId, Guid userId);
        Task<int> CountLearnedAsync(Guid moduleId, Guid userId);
        Task<int> CountLast7DaysCompletedLessonsAsync(Guid userId);
        Task<UserLessonProgress?> GetActiveUserLessonProgressByUserIdAsync(Guid userId);
        Task<Lesson?> GetActiveLessonByUserIdAsync(Guid userId, Guid CourseId);
        public Task<List<Lesson>> GetLearnedLessons(Guid userId, Guid courseId);
        public Task<List<string>> GetTitleLearnedLessons(Guid userId);
        public Task<bool> CreateNewLessonProgress(Guid userId, Guid lessonId);
        public Task<bool> CompleteLessons(Guid userId, Guid lessonId);
        public Task<bool> LearnedLessons(Guid userId, Guid lessonId);
    }
}
