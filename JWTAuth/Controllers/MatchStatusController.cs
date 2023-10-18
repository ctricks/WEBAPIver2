using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WEBAPI.Helpers;
using WEBAPI.Models.MatchStatus;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MatchStatusController : ControllerBase
    {
        private IMatchStatusService _matchstatusService;
        private readonly AppSettings _appSettings;

        public MatchStatusController(
            IMatchStatusService matchStatusService,
            IOptions<AppSettings> appSettings)
        {
            _matchstatusService = matchStatusService;
            _appSettings = appSettings.Value;
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("CreateStatus")]
        public IActionResult Register(UpdateRequest model)
        {
            _matchstatusService.Create(model);
            return Ok(new { message = "Match Status created successful" });
        }

        [AllowAnonymous]
        [HttpGet("SetDefaultValues")]
        public IActionResult SetDefault()
        {
            _matchstatusService.SetDefault();
            return Ok(new { message = "Default Status successfully added" });
        }

        [AllowAnonymous]
        [HttpGet("Lists")]
        public IActionResult GetAll()
        {
            var matchstatus = _matchstatusService.GetAll();
            return Ok(matchstatus);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _matchstatusService.GetById(id);
            return Ok(user);
        }
        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateRequest model)
        {
            _matchstatusService.Update(id, model);
            return Ok(new { message = "Match Status updated successfully" });
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _matchstatusService.Delete(id);
            return Ok(new { message = "Match Status deleted successfully" });
        }
    }
}
