using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface ISubscriptionPlanRepository : IBaseRepository<SubscriptionPlan> {
        IEnumerable<SubscriptionPlan> GetAllSubscriptionPlans();

    }
}
