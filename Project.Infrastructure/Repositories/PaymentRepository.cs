using Microsoft.EntityFrameworkCore;
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
    }
}
