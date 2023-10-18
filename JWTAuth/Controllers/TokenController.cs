using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WEBAPI.Helpers;
using WEBAPI.Models.Users;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService)
        {            
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));            
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public IActionResult Refresh(TokenRequest tokenrequest)
        {
            if (tokenrequest is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenrequest.AccessToken;
            string refreshToken = tokenrequest.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            TokenResponse response = _tokenService.Refresh(tokenrequest);
            
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("RefreshAdminToken")]
        public IActionResult RefreshAdminToken(TokenRequest tokenrequest)
        {
            if (tokenrequest is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenrequest.AccessToken;
            string refreshToken = tokenrequest.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

            TokenResponse response = _tokenService.RefreshAdmin(tokenrequest);

            return Ok(response);
        }

    }
}
