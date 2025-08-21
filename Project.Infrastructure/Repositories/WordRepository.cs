using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class WordRepository : BaseRepository<Word>, IWordRepository {
        public WordRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Word>> GetWordByLessonId(Guid lessonId) {
            var result = await _dbContext.Words
                .Where(x => x.LessonId == lessonId)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

        public async Task<List<Word>> GetAllWords() {
            var result = await _dbContext.Words
                .Include(x => x.Lesson)
                .Include(x => x.Lesson != null ? x.Lesson.Module : null)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }

        public async Task<Word?> GetWordById(Guid id) {
            var result = await _dbContext.Words
                .Include(x => x.Lesson)
                .FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
    }
}
