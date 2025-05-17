using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class WordQuizRepository : BaseRepository<WordQuiz>, IWordQuizRepository {
        public WordQuizRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
