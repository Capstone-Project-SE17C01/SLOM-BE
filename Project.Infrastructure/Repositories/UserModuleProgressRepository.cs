using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserModuleProgressRepository : BaseRepository<UserModuleProgress>, IUserModuleProgressRepository {
        public UserModuleProgressRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
