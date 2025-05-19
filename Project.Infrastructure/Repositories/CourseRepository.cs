using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class CourseRepository : BaseRepository<Course>, ICourseRepository {
        public CourseRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Course>> GetAllCourse() {
            var listCourse = await _dbContext.Courses
                .Where(x => x.IsPublished)
                .Include(x => x.Modules)
                .AsNoTracking()
                .ToListAsync();
            throw new NotImplementedException();
        }
    }
}
