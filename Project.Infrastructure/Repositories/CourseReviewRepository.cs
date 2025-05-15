using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class CourseReviewRepository : BaseRepository<CourseReview>, ICourseReviewRepository {
        public CourseReviewRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
