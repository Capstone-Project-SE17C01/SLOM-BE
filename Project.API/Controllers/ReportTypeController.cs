using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ReportTypeController : ControllerBase {
        private readonly IReportTypeRepository _reportTypeRepo;

        public ReportTypeController(IReportTypeRepository reportTypeRepo) {
            _reportTypeRepo = reportTypeRepo;
        }

        [HttpGet("GetAllReportType")]
        public async Task<IActionResult> GetAllAsync() {
            var result = await _reportTypeRepo.GetAll();
            return Ok(new APIResponse { result = result });
        }
    }
}
