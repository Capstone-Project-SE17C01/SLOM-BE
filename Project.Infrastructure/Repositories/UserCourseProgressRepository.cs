using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserCourseProgressRepository : BaseRepository<UserCourseProgress>, IUserCourseProgressRepository {
        public UserCourseProgressRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
