using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.ModuleDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("GetModuleById")]
        public async Task<APIResponse> GetModuleById(Guid id) {
            Module? module = await _moduleRepository.GetById(id);
            if (module == null) {
                return new APIResponse() { errorMessages = new List<string> { "Module not found" } };
            }
            return new APIResponse() { result = module };
        }

        [HttpGet]
        public async Task<APIResponse> GetAllModules() {
            try {
                var modules = await _moduleRepository.GetAllModuleHasCourse();
                return new APIResponse { result = modules };
            }
            catch (Exception ex) {
                return new APIResponse { errorMessages = new List<string> { ex.Message }, result = null };
            }
        }

        [HttpPost]
        public async Task<APIResponse> CreateModule([FromBody] ModuleRequestDTO module) {
            if (module == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid module data" } };
            }
            Module newModule = new Module {
                Id = module.Id,
                CourseId = module.CourseId,
                Title = module.Title,
                Description = module.Description,
                OrderNumber = module.OrderNumber,
                CreatedAt = DateTime.UtcNow
            };
            await _moduleRepository.Create(newModule);
            return new APIResponse() { result = newModule };
        }

        [HttpPut]
        public async Task<APIResponse> UpdateModule([FromBody] ModuleRequestDTO module) {
            if (module == null) {
                return new APIResponse() { errorMessages = new List<string> { "Invalid module data" } };
            }
            Module? existingModule = await _moduleRepository.GetById(module.Id);
            if (existingModule == null) {
                return new APIResponse() { errorMessages = new List<string> { "Module not found" } };
            }
            existingModule.Title = module.Title;
            existingModule.Description = module.Description;
            existingModule.OrderNumber = module.OrderNumber;
            await _moduleRepository.Update(existingModule);
            return new APIResponse() { result = existingModule };
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteModule(Guid id) {
            Module? module = await _moduleRepository.GetByIdForDelete(id);
            // Check if the module exists and has no lessons associated with it
            if (module != null) {

                if (module.Lessons != null && module.Lessons.Any()) {
                    return new APIResponse() { errorMessages = new List<string> { "Cannot delete module with associated lessons" } };
                }
            }
            if (module == null) {
                return new APIResponse() { errorMessages = new List<string> { "Module not found" } };
            }
            await _moduleRepository.Delete(module);
            return new APIResponse() { result = "Module deleted successfully" };
        }
    }
}
