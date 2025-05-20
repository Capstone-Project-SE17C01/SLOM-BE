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
    }
}
