using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.LanguageDTOs;
using Project.Core.Entities.Business.DTOs.ProfileDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/Profile")]
    [ApiController]
    public class ProfileController : ControllerBase {
        private readonly IProfileRepository _profileRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IBaseMapper<Profile, ProfileByEmailResponse> _mapper;


        public ProfileController(IProfileRepository profileRepository, IBaseMapper<Profile, ProfileByEmailResponse> mapper, ILanguageRepository languageRepository) {
            _profileRepository = profileRepository;
            _mapper = mapper;
            _languageRepository = languageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileById(string email) {

            var profile = await _profileRepository.GetProfileByEmail(email);
            if (profile == null) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "Profile not found" }
                });
            }
            var profileByEmailResponse = _mapper.MapModel(profile);

            return Ok(new APIResponse {
                result = profileByEmailResponse,
            });
        }

        [HttpGet("GetProfileByName")]
        public async Task<List<ProfileByNameResponse?>> GetProfilesByName(string input, string currentUserEmail) {
            return await _profileRepository.GetProfileByName(input, currentUserEmail);
        }

        [HttpPost("ChangeLanguage")]
        public async Task<IActionResult> ChangeLanguage([FromBody] ChangeLanguageRequestDTO request) {
            var profile = await _profileRepository.GetProfileByEmail(request.email);
            if (profile == null) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "Profile not found" }
                });
            }
            try {
                Language referredLanguage = await _languageRepository.GetLanguageByCodeAsync(request.newLanguageCode);
                profile.PreferredLanguageId = referredLanguage.Id;
                await _profileRepository.Update(profile);
                return Ok(new APIResponse {
                    result = new ChangeLanguageResponseDTO {
                        LanguageId = referredLanguage.Id,
                        LanguageCode = request.newLanguageCode,
                    }
                });
            } catch (Exception) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "ChangeLanguageFailed" }
                });
            }
        }
    }
}
