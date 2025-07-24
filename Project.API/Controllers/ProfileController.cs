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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProfiles() {
            var profiles = await _profileRepository.GetAll();
            if (profiles == null || !profiles.Any()) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "No profiles found" }
                });
            }
            var profileResponses = profiles.Select(profile => _mapper.MapModel(profile)).ToList();
            return Ok(new APIResponse {
                result = profileResponses
            });
        }

        [HttpGet("GetProfileByName")]
        public async Task<List<ProfileByNameResponse>> GetProfilesByName(string input, string currentUserEmail) {
            var profiles = await _profileRepository.GetProfileByName(input, currentUserEmail);
            return profiles.Where(profile => profile != null).ToList();
        }

        [HttpPost("ChangeLanguage")]
        public async Task<IActionResult> ChangeLanguage([FromBody] ChangeLanguageRequestDTO request) {
            var profile = await _profileRepository.GetProfileByEmail(request.Email);
            if (profile == null) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "Profile not found" }
                });
            }
            try {
                Language referredLanguage = await _languageRepository.GetLanguageByCodeAsync(request.NewLanguageCode);
                profile.PreferredLanguageId = referredLanguage.Id;
                await _profileRepository.Update(profile);
                return Ok(new APIResponse {
                    result = new ChangeLanguageResponseDTO {
                        LanguageId = referredLanguage.Id,
                        LanguageCode = request.NewLanguageCode,
                    }
                });
            } catch (Exception) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "ChangeLanguageFailed" }
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateModel profile) {
            if (profile == null || string.IsNullOrEmpty(profile.Email)) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "Invalid profile data" }
                });
            }
            try {
                var existingProfile = await _profileRepository.GetProfileByEmail(profile.Email);
                if (existingProfile == null) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "Profile not found" }
                    });
                }
                existingProfile.Username = profile.Username;
                existingProfile.AvatarUrl = profile.AvatarUrl;
                existingProfile.Bio = profile.Bio;
                existingProfile.Location = profile.Location;
                existingProfile.UpdatedAt = DateTime.UtcNow;
                await _profileRepository.Update(existingProfile);
                return Ok(new APIResponse {
                    result = _mapper.MapModel(existingProfile)
                });
            } catch (Exception ex) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { ex.Message }
                });
            }
        }
        [HttpPut("EditUpdateAt")]
        public async Task<IActionResult> EditUpdateAt(string email) {
            try {
                if (string.IsNullOrEmpty(email)) {
                    return BadRequest(new APIResponse {
                        errorMessages = new List<string> { "Email cannot be null or empty" }
                    });
                }
                bool result = await _profileRepository.EditUpdateAt(email);
                if (!result) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "Profile not found" }
                    });
                }
                return Ok(new APIResponse {
                    result = "Profile updated successfully"
                });
            } catch (Exception ex) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(Guid id) {
            try {
                var profile = await _profileRepository.GetById(id);
                if (profile == null) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "Profile not found" }
                    });
                }
                await _profileRepository.Delete(profile);
                return Ok(new APIResponse {
                    result = "Profile deleted successfully"
                });
            } catch (Exception ex) {
                return StatusCode(500, new APIResponse {
                    errorMessages = new List<string> { ex.Message }
                });
            }
        }
    }
}
