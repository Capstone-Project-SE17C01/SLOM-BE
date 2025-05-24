using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IUserCourseProgressRepository : IBaseRepository<UserCourseProgress> {
        Task<int> CountCompletedAsync(Guid courseId, Guid userId);
        Task<int> CountLast7DaysCompletedCoursesAsync(Guid userId);
        Task<List<Course>> GetCoursesByUserIdAsync(Guid userId);
    }
}
