using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class ModuleRepository : BaseRepository<Module>, IModuleRepository {
        public ModuleRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Module>> GetModuleByCourseId(Guid courseId) {
            var result = await _dbContext.Modules.Where(x => x.CourseId == courseId).Include(x => x.Lessons).ToListAsync();
            return result;
        }
    }
}
