using Project.Core.Entities.Business.DTOs.Profile;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IProfileRepository : IBaseRepository<Profile> {
        Task<Profile?> GetProfileByEmail(string email);
        Task<Profile?> GetProfileByUserName(string userName);
        Task<List<ProfileByNameResponse?>> GetProfileByName(string name, string currentUserName);
    }
}
