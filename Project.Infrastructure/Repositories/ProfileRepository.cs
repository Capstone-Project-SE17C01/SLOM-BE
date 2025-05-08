using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Business.DTOs.ProfileDTOs;
using Project.Core.Entities.General;
using Project.Core.Exceptions;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class ProfileRepository : BaseRepository<Profile>, IProfileRepository {
        public ProfileRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<Profile?> GetProfileByEmail(string email) {
            var profile = await _dbContext.Profiles.FirstOrDefaultAsync(p => p.Email == email);
            if (profile == null) {
                throw new NotFoundException("Profile not found with the provided email.");
            }
            return profile;
        }

        public async Task<List<ProfileByNameResponse>> GetProfileByName(string name, string currentUserEmail) {
            if (string.IsNullOrEmpty(name)) {
                return new List<ProfileByNameResponse>();
            }
            return await _dbContext.Profiles
                .Select(x => new ProfileByNameResponse { UserAvatar = x.AvatarUrl ?? "", UserName = x.Username ?? "", UserEmail = x.Email ?? "" })
                .Where(x => x.UserName.Contains(name) && x.UserEmail != currentUserEmail).Take(5).ToListAsync();
        }
    }
}
