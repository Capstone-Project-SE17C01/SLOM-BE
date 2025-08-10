using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IWordRepository : IBaseRepository<Word> {
        public Task<List<Word>> GetWordByLessonId(Guid lessonId);
        public Task<List<Word>> GetAllWords();
        public Task<Word?> GetWordById(Guid id);
    }
}
