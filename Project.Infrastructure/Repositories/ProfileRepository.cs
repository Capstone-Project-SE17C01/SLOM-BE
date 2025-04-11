using Microsoft.EntityFrameworkCore;
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
    }
}
