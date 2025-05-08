using Project.Core.Entities.Business.DTOs.ProfileDTOs;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IProfileRepository : IBaseRepository<Profile> {
        Task<Profile?> GetProfileByEmail(string email);
        Task<List<ProfileByNameResponse>> GetProfileByName(string name, string currentUserEmail);
    }
}
