using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POSIMSWebApi.Auth;
using POSIMSWebApi.AuthServices;
using POSIMSWebApi.Dtos.AuthDtos;

namespace Training.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCredentialController : ControllerBase
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUserAuthenticationServices _userAuthenticationServices;

        public UserCredentialController(
            UserManager<ApplicationIdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IUserAuthenticationServices userAuthenticationServices
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _userAuthenticationServices = userAuthenticationServices;
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<string>> RegisterUser(RegisterUser user)
        {
            var results = await _userAuthenticationServices.RegisterUser(user);
            return Ok(results);
        }

        [HttpPost("RegisterAdmin")]
        public async Task<ActionResult<string>> RegisterAdmin(RegisterUser user)
        {
            var results = await _userAuthenticationServices.RegisterAdmin(user);
            return Ok(results);
        }

        [HttpPost("LoginAccount")]
        public async Task<ActionResult<string>> LoginUser(RegisterUser user)
        {
            var results = await _userAuthenticationServices.LoginAccount(user);
            return Ok(results);
        }
    }
}
