using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class QuizRepository : BaseRepository<Quiz>, IQuizRepository {
        public QuizRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Quiz>> GetAllQuizByLessonId(Guid lessonId) {
            var result = await _dbContext.Quizzes
                .Where(x => x.LessonId == lessonId)
                .Include(x => x.WordQuizzes)
                .Include(x => x.QuizOptions)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }
        public async Task<int> CountAsyncByCourseId(Guid courseId) {
            var count = await _dbContext.Quizzes
                .Where(q => q.Lesson != null
                         && q.Lesson.Module != null
                         && q.Lesson.Module.CourseId == courseId)
                .GroupBy(q => q.LessonId)
                .CountAsync();
            return count;
        }

        public async Task<List<Quiz>> GetAllQuiz() {
            var result = await _dbContext.Quizzes
                .Include(x => x.Lesson)
                .Include(x => x.Lesson != null ? x.Lesson.Module : null)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }
    }
}
