using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IQuizRepository : IBaseRepository<Quiz> {
        public Task<List<Quiz>> GetAllQuizByLessonId(Guid lessonId);
    }
}
