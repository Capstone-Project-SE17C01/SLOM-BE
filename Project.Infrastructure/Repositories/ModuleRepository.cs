using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class ModuleRepository : BaseRepository<Module>, IModuleRepository {
        public ModuleRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
