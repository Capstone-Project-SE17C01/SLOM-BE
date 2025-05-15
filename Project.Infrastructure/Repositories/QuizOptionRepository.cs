using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class QuizOptionRepository : BaseRepository<QuizOption>, IQuizOptionRepository {
        public QuizOptionRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
