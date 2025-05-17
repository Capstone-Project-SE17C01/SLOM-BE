using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserLessonProgressRepository : BaseRepository<UserLessonProgress>, IUserLessonProgressRepository {
        public UserLessonProgressRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
