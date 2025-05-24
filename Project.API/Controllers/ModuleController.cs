using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase {
        private readonly IModuleRepository _moduleRepository;

        public ModuleController(ApplicationDbContext context) {
            _moduleRepository = new ModuleRepository(context);
        }

        [HttpGet("AllModules")]
        public async Task<List<Module>> GetOngoingUserLesson(Guid courseId) {
            return await _moduleRepository.GetModuleByCourseId(courseId);
        }
    }
}
