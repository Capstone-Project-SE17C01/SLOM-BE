using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Core.Exceptions;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Repositories {
    public class RoleRepository : BaseRepository<Role>, IRoleRepository {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext) {
        }

        public async Task<Role> GetRoleByNameAsync(string name) {
            var role = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);

            if (role == null) {
                throw new NotFoundException("Role not found");
            }

            return role;
        }
        public async Task<Guid> GetIdByNameAsync(string name) {
            var role = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name.ToUpper() == name.ToUpper());

            if (role == null) {
                throw new NotFoundException("Role not found");
            }
            return role.Id;
        }
    }
}
