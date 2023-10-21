using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.Users;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IUserAdminService _useradminService;
        private readonly ITokenService _tokenService;
        private readonly AppSettings _appSettings;
        

        public UsersController(
            IUserService userService,
            IUserAdminService useradminService,
            ITokenService tokenService,
            IOptions<AppSettings> appSettings)
        {
            _useradminService = useradminService;
            _userService = userService;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _appSettings = appSettings.Value;
        }

        [Authorize(Roles = "Admin,user")]
        [HttpGet("isExpireToken")]
        public IActionResult CheckExipryBearerToken()
        {
            //Get Username via http (authorize user)
            var Username = HttpContext.User.Identities.FirstOrDefault().Name.ToString();

            if (Username == null) throw new Exception("Invalid Token Bearer. Please check");

            var useradmin = _useradminService.CheckTokenByBearerToken(Username);

            return Ok(useradmin);
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            //var response = _userService.Authenticate(model);

            ////For Refresh Token
            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.MobilePhone, model.PhoneNumber),
            //    new Claim(ClaimTypes.Role, response.Role),
            //    new Claim("id", response.Id.ToString())
            //};

            //var accessToken = _tokenService.GenerateAccessToken(claims);
            //var refreshToken = _tokenService.GenerateRefreshToken();

            //string MinuteExpire = _appSettings.MinuteExpire;

            //DateTime ExpiredToken = DateTime.Now;

            //if (MinuteExpire == null)
            //{
            //    ExpiredToken = DateTime.Now.AddMinutes(5);
            //}else
            //{
            //    ExpiredToken = DateTime.Now.AddMinutes(double.Parse(MinuteExpire.ToString()));
            //}

            //_userService.updateUserToken(model.PhoneNumber,accessToken, refreshToken,ExpiredToken);

            //response.Token = accessToken;
            //response.RefreshToken = refreshToken;
            //response.ExpireToken = ExpiredToken;

            //return Ok(response);
            var response = _userService.Authenticate(model);

            var refreshToken = _tokenService.GenerateRefreshToken();

            response.RefreshToken = refreshToken;

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { message = "Registration successful" });
        }

        [Authorize(Roles = "user")]
        [HttpGet("Lists")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }


        [Authorize(Roles = "user")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);

            return Ok(user);
        }

        [Authorize(Roles = "user")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateRequest model)
        {
            _userService.Update(id, model);
            return Ok(new { message = "User updated successfully" });
        }

        
        [Authorize(Roles = "user")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok(new { message = "User deleted successfully" });
        }

        [AllowAnonymous]
        [HttpPut("Logout/{TokenId}")]
        public IActionResult Logout(string TokenId)
        {
            _userService.Logout(TokenId);
            return Ok(new { message = "User successfully logout" });
        }
    }
}
