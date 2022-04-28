using System;
using System.Threading.Tasks;
using eCommerceApi.Domain.Interfaces;
using eCommerceApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IApplicationDbContext _dbContext { get; }
        private IUserService _userService;
        private ILogger<AccountController> _logger;

        public AccountController(IApplicationDbContext dbContext, IUserService userService, ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto user)
        {
            try
            {
                var userData = await _userService.Authenticate(user.LoginID, user.Password);

                if (userData == null)
                {
                    return Unauthorized(new { message = "Invalid Login or Password" });
                }

                return Ok(userData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
                                                
        }
    }
}