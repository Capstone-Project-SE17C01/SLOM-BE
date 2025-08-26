using Project.Core.Entities.Business.DTOs.AdminDTOs;
using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IPaymentRepository : IBaseRepository<Payment> {
        Task<Payment> GetPaymentByOrderCodeAsync(int orderCode);
        Task<List<Payment>> GetListPaymentByUserIdAsync(Guid userId);
        Task<decimal> GetTotalRevenueAsync();
        Task<List<TimeSeriesItem<decimal>>> GetRevenueStatsAsync();
        Task<List<Payment>> GetAllPaymentAndReportAsync();

    }
}
