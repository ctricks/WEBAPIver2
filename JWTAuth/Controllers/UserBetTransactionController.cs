using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WEBAPI.Helpers;
using WEBAPI.Models.BetTransaction;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserBetTransactionController :ControllerBase
    {
        private IUserService _userService;
        private IBetTransactionService _bettransService;
        private readonly ITokenService _tokenService;        
        private readonly AppSettings _appSettings;

        public UserBetTransactionController(
            IUserService userService,
            IBetTransactionService bettransService,
            ITokenService tokenService,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _bettransService = bettransService;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _appSettings = appSettings.Value;
        }


        [AllowAnonymous]
        [HttpPost("BetUser")]
        public IActionResult Register(BetTransactionRequest model)
        {
            _bettransService.Create(model);
            return Ok(new { message = "Bet successfully added" });
        }

        [AllowAnonymous]
        [HttpGet("Lists")]
        public IActionResult GetAll()
        {
            var userbet = _bettransService.GetAll();
            if (userbet == null) throw new AppException("No Fight Found. Please check if first");
            return Ok(userbet);
        }


        [AllowAnonymous]
        [HttpGet("GetAllBetByFightNumber/{FightNumber}")]
        public IActionResult GetBetByFightNumber(int FightNumber,DateTime MatchDate)
        {
            var userbet = _bettransService.GetBetUserByFightNumber(FightNumber,MatchDate);

            if (userbet == null) throw new AppException("No Transaction on this fight. Please check if first");

            return Ok(userbet);
        }

        [AllowAnonymous]
        [HttpGet("GetAllBetByFightColor{FightNumber}")]
        public IActionResult GetBetColorByFightNumber(int FightNumber, DateTime MatchDate)
        {
            var userbet = _bettransService.GetBetColorByFightNumber(FightNumber, MatchDate);

            if (userbet == null) throw new AppException("No Transaction on this fight. Please check if first");

            return Ok(userbet);
        }

    }
}
