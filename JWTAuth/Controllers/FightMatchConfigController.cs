using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.FightMatch;
using WEBAPI.Models.FightMatchConfig;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FightMatchConfigController : ControllerBase
    {
        private IFightMatchConfigService _fightmatchconfigService;      
        private readonly AppSettings _appSettings;

        public FightMatchConfigController(
            IFightMatchConfigService fightmatchconfigService,            
            IOptions<AppSettings> appSettings)
        {
            _fightmatchconfigService = fightmatchconfigService;
            _appSettings = appSettings.Value;
        }

        //CB-For Token Authorization 
        //CB 10182023 - Adding authentication from FightMatchConfig if the match is alread start or not.
        [Authorize(Roles ="Admin")]
        [HttpPost("StartMatchConfig")]
        public IActionResult Register(FightMatchConfigRequest model)
        {
            _fightmatchconfigService.Start(model);
            return Ok(new { message = "Fight is start today : " + model.FightMatchDate });
        }

        //CB-For Token Authorization 
        //CB 10182023 - Adding authentication from FightMatchConfig if the match is alread start or not.
        [Authorize(Roles = "Admin")]
        [HttpPost("ClosedMatchConfig/{FightDate}")]
        public IActionResult Closed(DateTime FightDate)
        {
            _fightmatchconfigService.End(FightDate);
            return Ok(new { message = "Fight is now Close today : " + FightDate });
        }

        [AllowAnonymous]
        [HttpGet("Lists")]
        public IActionResult GetAll()
        {
            var fightMatchesConfig = _fightmatchconfigService.GetAll();
            return Ok(fightMatchesConfig);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var fightmatchConfig = _fightmatchconfigService.GetFightMatchById(id);
            return Ok(fightmatchConfig);
        }
        

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _fightmatchconfigService.Delete(id);
            return Ok(new { message = "Fight match config is deleted successfully" });
        }

    }
}