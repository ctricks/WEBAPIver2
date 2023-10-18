using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using WEBAPI.Helpers;
using WEBAPI.Models.BetColor;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BetColorController : ControllerBase
    {
        private IColorConfigService _colorconfigService;
        private readonly AppSettings _appSettings;

        public BetColorController(
            IColorConfigService colorConfigService,
            IOptions<AppSettings> appSettings)
        {
            _colorconfigService = colorConfigService;
            _appSettings = appSettings.Value;
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("CreateColor")]
        public IActionResult Register(UpdateRequest model)
        {
            _colorconfigService.Create(model);
            return Ok(new { message = "Match Result created successful" });
        }

        [AllowAnonymous]
        [HttpGet("SetDefaultValues")]
        public IActionResult SetDefault()
        {
            _colorconfigService.SetDefault();
            return Ok(new { message = "Default Color successfully added" });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Lists")]        
        public IActionResult GetAll()
        {
            var matchstatus = _colorconfigService.GetAll();
            return Ok(matchstatus);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _colorconfigService.GetById(id);
            return Ok(user);
        }
        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut()]
        public IActionResult Update(UpdateRequest model)
        {
            _colorconfigService.Update(model);
            return Ok(new { message = "Color Name updated successfully" });
        }

        //CB-For Token Authorization
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, string TokenId)
        {
            _colorconfigService.Delete(id,TokenId);
            return Ok(new { message = "Color Name deleted successfully" });
        }
    }
}
