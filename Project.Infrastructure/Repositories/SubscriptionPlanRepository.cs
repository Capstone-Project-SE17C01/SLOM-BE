using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class SubscriptionPlanRepository : BaseRepository<SubscriptionPlan>, ISubscriptionPlanRepository {
        public SubscriptionPlanRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
