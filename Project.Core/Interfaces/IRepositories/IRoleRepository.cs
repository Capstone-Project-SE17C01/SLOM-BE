using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IRoleRepository : IBaseRepository<Role> {
        Task<Role> GetRoleByNameAsync(string name);
        Task<Guid> GetIdByNameAsync(string name);
    }
}
