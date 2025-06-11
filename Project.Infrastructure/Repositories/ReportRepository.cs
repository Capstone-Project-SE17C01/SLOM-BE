using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class ReportRepository : BaseRepository<Report>, IReportRepository {
        public ReportRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }
    }
}
