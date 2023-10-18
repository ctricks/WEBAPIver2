using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WEBAPI.Helpers;
using WEBAPI.Models.WalletTransactions;
using WEBAPI.Services;

namespace WEBAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WalletTransactionsController : ControllerBase
    {
        private IWalletTransactionsService _wallettxnService;
        private readonly AppSettings _appSettings;

        public WalletTransactionsController(
            IWalletTransactionsService wallettxnService,
            IOptions<AppSettings> appSettings)
        {
            _wallettxnService = wallettxnService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _wallettxnService.Authenticate(model);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("NewUserWallet")]
        public IActionResult NewWallet(AuthenticateRequest model)
        {
            
            return Ok(_wallettxnService.newUserWallet(model));
        }

        [AllowAnonymous]
        [HttpPost("InsertTransaction")]
        public IActionResult Transaction(RegisterRequest model)
        {
            _wallettxnService.Create(model);
            return Ok(new { message = "Wallet Transaction successfully saved" });
        }

        [AllowAnonymous]
        [HttpGet("Lists")]
        public IActionResult GetAll()
        {
            var wallettxn = _wallettxnService.GetAll();
            return Ok(wallettxn);            
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _wallettxnService.GetById(id);
            return Ok(user);
        }

        ////[AllowAnonymous]
        ////[HttpPut("{id}")]
        ////public IActionResult Update(int id, UpdateRequest model)
        ////{
        ////    _userService.Update(id, model);
        ////    return Ok(new { message = "User updated successfully" });
        ////}

        //[AllowAnonymous]
        //[HttpDelete("{id}")]
        //public IActionResult Delete(int id)
        //{
        //    _wallettxnService.Delete(id);
        //    return Ok(new { message = "Transaction deleted successfully" });
        //}
    }
}
