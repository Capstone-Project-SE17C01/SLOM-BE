using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class ProfileRepository : BaseRepository<Profile>, IProfileRepository {
        public ProfileRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

    }
}
