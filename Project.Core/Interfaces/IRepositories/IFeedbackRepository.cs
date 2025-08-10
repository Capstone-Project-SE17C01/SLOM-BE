using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface IFeedbackRepository {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> GetByIdAsync(int id);
        Task<Feedback> AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(int id);
    }
}
