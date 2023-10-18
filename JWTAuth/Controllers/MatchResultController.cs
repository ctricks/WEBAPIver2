using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WEBAPI.Helpers;
using WEBAPI.Models.MatchResult;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MatchResultController : ControllerBase
    {
        private IMatchResultService _matchresultService;
        private readonly AppSettings _appSettings;

        public MatchResultController(
            IMatchResultService matchResultService,
            IOptions<AppSettings> appSettings)
        {
            _matchresultService = matchResultService;
            _appSettings = appSettings.Value;
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("CreateResult")]
        public IActionResult Register(UpdateRequest model)
        {
            _matchresultService.Create(model);
            return Ok(new { message = "Match Result created successful" });
        }

        [AllowAnonymous]
        [HttpGet("SetDefaultValues")]
        public IActionResult SetDefault()
        {
            _matchresultService.SetDefault();
            return Ok(new { message = "Default Result successfully added" });
        }

        [AllowAnonymous]
        [HttpGet("Lists")]
        public IActionResult GetAll()
        {
            var matchstatus = _matchresultService.GetAll();
            return Ok(matchstatus);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _matchresultService.GetById(id);
            return Ok(user);
        }
        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateRequest model)
        {
            _matchresultService.Update(id, model);
            return Ok(new { message = "Match Result updated successfully" });
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _matchresultService.Delete(id);
            return Ok(new { message = "Match Status deleted successfully" });
        }
    }
}
