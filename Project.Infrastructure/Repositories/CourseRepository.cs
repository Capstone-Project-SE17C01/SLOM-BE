using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class CourseRepository : BaseRepository<Course>, ICourseRepository {
        public CourseRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
