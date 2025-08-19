using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IModuleRepository : IBaseRepository<Module> {
        public Task<List<Module>> GetModuleByCourseId(Guid courseId);
        public Task<List<Module>> GetAllModuleHasCourse();
        public Task<Module?> GetByIdForDelete(Guid moduleId);
        public Task<int> CountAsyncByCourseId(Guid courseId);
    }
}
