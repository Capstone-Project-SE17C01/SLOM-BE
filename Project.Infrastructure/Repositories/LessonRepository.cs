using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class LessonRepository : BaseRepository<Lesson>, ILessonRepository {
        public LessonRepository(ApplicationDbContext dbContext) : base(dbContext) {

        }

        public async Task<List<Lesson>> GetAllLessonHasModule() {
            var result = await _dbContext.Lessons
                .Include(x => x.Module)
                .OrderBy(x => x.OrderNumber)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

        public async Task<Lesson?> GetByIdForDelete(Guid lessonId) {
            var result = await _dbContext.Lessons
                .Include(x => x.Words)
                .Include(x => x.Quizzes)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == lessonId);
            return result;
        }

        public async Task<List<Lesson>> GetLessonByModuleId(Guid moduleId) {
            var result = await _dbContext.Lessons
                .Where(x => x.ModuleId == moduleId)
                .Include(x => x.Module)
                .OrderBy(x => x.OrderNumber)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

    }
}
