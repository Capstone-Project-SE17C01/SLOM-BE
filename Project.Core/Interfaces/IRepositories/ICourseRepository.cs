using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface ICourseRepository : IBaseRepository<Course> {
        public Task<List<Course>> GetAllCourse();
    }
}
