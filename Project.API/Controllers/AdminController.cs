using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.AdminDTOs;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IPaymentRepository _paymentRepository;

        public AdminController(IMeetingRepository meetingRepository, ICourseRepository courseRepository, IProfileRepository profileRepository, IPaymentRepository paymentRepository) {
            _meetingRepository = meetingRepository;
            _courseRepository = courseRepository;
            _profileRepository = profileRepository;
            _paymentRepository = paymentRepository;
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary() {
            try {
                var totalUsers = await _profileRepository.CountAsync();
                var totalCourses = await _courseRepository.CountAsync();
                var totalMeetings = await _meetingRepository.CountActiveMeetingsAsync();
                var totalRevenue = await _paymentRepository.GetTotalRevenueAsync();

                var userToday = await _profileRepository.CountUsersInUseTodayAsync();

                List<TimeSeriesItem<int>> newUserStats = await _profileRepository.GetNewUserStatsAsync();

                var revenueStats = await _paymentRepository.GetRevenueStatsAsync();
                decimal totalRevenueToday = revenueStats
                    .Where(x => x.Date.Date == DateTime.UtcNow.Date)
                    .Sum(x => x.Value);
                int totalNewUsersToday = newUserStats
                    .Where(x => x.Date.Date == DateTime.UtcNow.Date)
                    .Sum(x => x.Value);

                var summary = new {
                    TotalUsers = totalUsers,
                    TotalCourses = totalCourses,
                    TotalMeetings = totalMeetings,
                    TotalRevenue = totalRevenue,
                    UserInUseToday = userToday,
                    NewUserStats = newUserStats,
                    RevenueStats = revenueStats,
                    RevenueToday = totalRevenueToday,
                    NewUserToday = totalNewUsersToday
                };

                return Ok(new APIResponse { result = summary });
            }
            catch (Exception) {
                return StatusCode(500, new APIResponse { errorMessages = new List<string> { "Server Error" } });
            }
        }
    }
}
