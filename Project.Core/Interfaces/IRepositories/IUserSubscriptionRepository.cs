using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IUserSubscriptionRepository : IBaseRepository<UserSubscription> {
        Task<UserSubscription> GetUserSubscriptionByBothIdAsync(Guid? userId, Guid? planId);
    }
}
