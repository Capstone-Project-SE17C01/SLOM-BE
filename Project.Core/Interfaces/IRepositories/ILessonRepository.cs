using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface ILessonRepository : IBaseRepository<Lesson> {
        public Task<List<Lesson>> GetAllLessonHasModule();
        public Task<Lesson?> GetByIdForDelete(Guid lessonId);
    }
}
