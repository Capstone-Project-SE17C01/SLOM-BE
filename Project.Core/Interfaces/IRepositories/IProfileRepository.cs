using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IProfileRepository : IBaseRepository<Profile> {
        Task<Profile?> GetProfileByEmail(string email);
    }
}
