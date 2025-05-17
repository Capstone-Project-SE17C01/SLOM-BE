using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class QuizRepository : BaseRepository<Quiz>, IQuizRepository {
        public QuizRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
