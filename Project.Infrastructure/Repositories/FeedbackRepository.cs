using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class FeedbackRepository : IFeedbackRepository {
        private readonly ApplicationDbContext _context;

        public FeedbackRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync() {
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<Feedback> GetByIdAsync(int id) {
            return await _context.Feedbacks.FindAsync(id);
        }

        public async Task<Feedback> AddAsync(Feedback feedback) {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task UpdateAsync(Feedback feedback) {
            _context.Entry(feedback).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null) {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }
    }
}
