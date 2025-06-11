using Microsoft.AspNetCore.Mvc;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.ReportDTOs;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;

namespace Project.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase {
        private readonly IReportRepository _reportRepo;
        private readonly IBaseMapper<CreateReportRequestDTO, Report> _mapper;


        public ReportController(IReportRepository reportRepo, IBaseMapper<CreateReportRequestDTO, Report> mapper) {
            _reportRepo = reportRepo;
            _mapper = mapper;
        }

        [HttpPost("CreateReport")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportRequestDTO createRequest) {
            if (createRequest == null) {
                return BadRequest(new { error = "Invalid report data." });
            }

            try {
                var result = await _reportRepo.Create(_mapper.MapModel(createRequest));
                if (result == null) {
                    return StatusCode(500, new { error = "Failed to create report." });
                }
                return Ok(new APIResponse { result = "Created successfully" });
            } catch (Exception ex) {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
