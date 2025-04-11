using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/Profile")]
    [ApiController]
    public class ProfileController : ControllerBase {
        private readonly IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository) {
            _profileRepository = profileRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileById(string email) {

            var profile = await _profileRepository.GetProfileByEmail(email);
            return Ok(new APIResponse {
                result = profile,
            });
        }
    }
}
