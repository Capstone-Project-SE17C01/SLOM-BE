using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Exceptions;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;


namespace Project.Infrastructure.Repositories {
    public class LanguageRepository : BaseRepository<Language>, ILanguageRepository {
        public LanguageRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<Language> GetLanguageByIdAsync(Guid id) {
            var language = await _dbContext.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (language == null) {
                throw new NotFoundException("Language not found");
            }

            return language;
        }

        public async Task<Language> GetLanguageByCodeAsync(string code) {
            var language = await _dbContext.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Code == code);

            if (language == null) {
                throw new NotFoundException("Language not found");
            }

            return language;
        }

        public async Task<Guid> GetIdByCodeAsync(string code) {
            var language = await _dbContext.Languages
                .AsNoTracking()
               .FirstOrDefaultAsync(l => l.Code.ToUpper() == code.ToUpper());

            if (language == null) {
                throw new NotFoundException("Language not found");
            }

            return language.Id;
        }
    }
}
