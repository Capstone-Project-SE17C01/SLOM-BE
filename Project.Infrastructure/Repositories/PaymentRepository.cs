using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Business.DTOs.AdminDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<Payment> GetPaymentByOrderCodeAsync(int orderCode) {
            var payment = await _dbContext.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);
            return payment ?? new Payment();
        }

        public async Task<List<Payment>> GetListPaymentByUserIdAsync(Guid userId) {
            var paymentInfos = await _dbContext.Payments
                .AsNoTracking()
                .Where(p => p.UserId == userId && p.Status != null && p.Status.Equals("PAID"))
                .ToListAsync();
            return paymentInfos;
        }

        public async Task<decimal> GetTotalRevenueAsync() {
            var totalRevenue = await _dbContext.Payments
                .Where(p => p.Status != null && p.Status.Equals("PAID"))
                .SumAsync(p => p.Amount);
            return totalRevenue;
        }

        public async Task<List<TimeSeriesItem<decimal>>> GetRevenueStatsAsync() {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var revenueStats = await _dbContext.Payments
                .Where(p => p.CreatedAt >= startOfMonth && p.CreatedAt <= endOfMonth && p.Status != null && p.Status.Equals("PAID"))
                .GroupBy(p => p.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new TimeSeriesItem<decimal> {
                    Date = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc),  // ép Kind ở đây
                    Value = g.Sum(p => p.Amount)
                })
                .ToListAsync();

            return revenueStats;
        }

        public async Task<List<Payment>> GetAllPaymentAndReportAsync() {
            var payment = await _dbContext.Payments
                .AsNoTracking()
                .ToListAsync();

            foreach (var item in payment) {
                item.Reports = await _dbContext.Reports
                    .AsNoTracking()
                    .Where(r => r.TransactionId == item.Id)
                    .ToListAsync();
            }
            return payment;
        }
    }
}
