using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class UserSubscriptionRepository : BaseRepository<UserSubscription>, IUserSubscriptionRepository {
        public UserSubscriptionRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
        public async Task<UserSubscription> GetUserSubscriptionByBothIdAsync(Guid? userId, Guid? planId) {

            var subscription = await _dbContext.UserSubscriptions
                .FirstOrDefaultAsync(x => x.UserId == userId && x.PlanId == planId);

            return subscription ?? throw new InvalidOperationException("User subscription not found.");
        }
    }
}
