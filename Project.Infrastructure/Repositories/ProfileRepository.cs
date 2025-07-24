using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Business.DTOs.AdminDTOs;
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
            var profile = await _dbContext.Profiles.Include(p => p.PreferredLanguage).FirstOrDefaultAsync(p => p.Email == email);
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

        public async Task<string?> GetRoleNameByEmailAsync(string email) {
            var profile = await _dbContext.Profiles
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.Email == email);

            return profile?.Role?.Name;
        }

        public async Task<int> CountUsersInUseTodayAsync() {
            var today = DateTime.UtcNow.Date;
            return await _dbContext.Profiles
                .CountAsync(p => p.UpdatedAt.Date == today);
        }
        public async Task<List<TimeSeriesItem<int>>> GetNewUserStatsAsync() {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await _dbContext.Profiles
                .Where(p => p.CreatedAt >= startOfMonth && p.CreatedAt <= endOfMonth)
                .GroupBy(p => p.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new TimeSeriesItem<int> {
                    Date = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc), // force UTC here
                    Value = g.Count()
                })
                .ToListAsync();
        }
        public async Task<bool> EditUpdateAt(string email) {
            var profile = await _dbContext.Profiles.FirstOrDefaultAsync(p => p.Email == email);
            if (profile == null) {
                throw new NotFoundException("Profile not found with the provided email.");
            }
            profile.UpdatedAt = DateTime.UtcNow;
            _dbContext.Profiles.Update(profile);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
