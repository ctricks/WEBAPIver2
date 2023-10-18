using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WEBAPI.Helpers;
using WEBAPI.Models.FightMatch;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FightMatchController : ControllerBase
    {
        private IFightMatchService _fightmatchService;      
        private readonly AppSettings _appSettings;

        public FightMatchController(
            IFightMatchService fightmatchService,            
            IOptions<AppSettings> appSettings)
        {
            _fightmatchService = fightmatchService;
            _appSettings = appSettings.Value;
        }

        //CB-For Token Authorization 
        //CB 10182023 - Adding authentication from FightMatchConfig if the match is alread start or not.
        [Authorize(Roles ="Admin")]
        [HttpPost("CreateMatch")]
        public IActionResult Register(FightMatchRequest model)
        {
            string Message = _fightmatchService.Register(model);
            if (Message.ToLower() == "ok")
            {
                return Ok(new { message = "Fight Match successfully created" });
            }
            else
            {
                return NotFound(new { message = Message });
            }
        }

        //CB 10182023 - Adding authentication from FightMatchConfig if the match is alread start or not.
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateMatch")]
        public IActionResult UpdateMatch(FightMatchRequest model)
        {
            string Message = _fightmatchService.UpdateMatch(model);
            if (Message.ToLower() == "ok")
            {
                return Ok(new { message = "Fight Match successfully updated" });
            }
            else
            {
                return NotFound(new { message = Message });
            }
        }

        //CB-For Token Authorization 
        //CB 10182023 - Adding authentication from FightMatchConfig if the match is alread start or not.
        [Authorize(Roles = "Admin")]
        [HttpPost("OpenMatch/{MatchDate}")]
        public IActionResult StartMatch(DateTime MatchDate)
        {
            var fighmatch = _fightmatchService.OpenMatch(MatchDate);
            if (fighmatch != null)
            {
                return Ok(fighmatch);
            }
            else
            {
                return NotFound(new { message = "Error: Cannot Open Match. Please check your date match in config table"});
            }
        }


        [AllowAnonymous]
        [HttpGet("Lists")]
        public IActionResult GetAll()
        {
            var fightMatches = _fightmatchService.GetAll();
            return Ok(fightMatches);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var fightmatch = _fightmatchService.GetFightMatchById(id);
            return Ok(fightmatch);
        }
        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, FightMatchRequest model)
        {
            _fightmatchService.Update(id, model);
            return Ok(new { message = "Fight match is updated successfully" });
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("GetLastMatchByDate/{MatchDate}")]
        public IActionResult LastMatchByDate(DateTime MatchDate)
        {
            var fightmatch = _fightmatchService.GetLastFightMatchByDate(MatchDate);
            if (fightmatch == null)
            {
                return NotFound(new { message = "Fight Match is not found. Please check your record or create a new one" });
            }
            else
            {
                return Ok(fightmatch);
            }
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _fightmatchService.Delete(id);
            return Ok(new { message = "Fight match is deleted successfully" });
        }

    }
}