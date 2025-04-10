using Project.Core.Entities.General;

namespace Project.Core.Interfaces.IRepositories {
    public interface ILanguageRepository : IBaseRepository<Language> {
        Task<Language> GetLanguageByIdAsync(Guid id);
        Task<Language> GetLanguageByCodeAsync(string code);
        Task<Guid> GetIdByCodeAsync(string code);
    }
}
