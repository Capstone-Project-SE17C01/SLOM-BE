using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class QuizAttemptRepository : BaseRepository<QuizAttempt>, IQuizAttemptRepository {
        public QuizAttemptRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
