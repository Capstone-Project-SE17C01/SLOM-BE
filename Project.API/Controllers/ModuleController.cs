using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase {
        private readonly IModuleRepository _moduleRepository;

        public ModuleController(IModuleRepository moduleRepository) {
            _moduleRepository = moduleRepository;
        }

        [HttpGet("AllModules")]
        public async Task<APIResponse> GetOngoingUserLesson(Guid courseId) {
            List<Module> modules = await _moduleRepository.GetModuleByCourseId(courseId);
            return new APIResponse() { result = modules };
        }
    }
}
