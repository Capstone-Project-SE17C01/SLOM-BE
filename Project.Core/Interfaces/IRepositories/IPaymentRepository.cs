using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IPaymentRepository : IBaseRepository<Payment> {
        Task<Payment> GetPaymentByOrderCodeAsync(int orderCode);
    }
}
