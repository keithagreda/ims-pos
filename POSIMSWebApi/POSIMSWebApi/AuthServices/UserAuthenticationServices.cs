using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using POSIMSWebApi.Auth;
using POSIMSWebApi.Dtos.AuthDtos;

namespace POSIMSWebApi.AuthServices
{
    public class UserAuthenticationServices : IUserAuthenticationServices
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserAuthenticationServices(
            UserManager<ApplicationIdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public async Task<string> RegisterUser(RegisterUser register)
        {
            try
            {
                var isUserExist = await _userManager.FindByNameAsync(register.UserName);
                if (isUserExist != null)
                {
                    return "User already exists";
                }
                var user = new ApplicationIdentityUser
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = register.UserName,
                    Email = "",
                };
                var result = await _userManager.CreateAsync(user, register.Password);
                if (!result.Succeeded)
                {
                    var errors = "";
                    foreach (var error in result.Errors)
                    {
                        errors = error.Code + ", " + error.Description;
                    }

                    return ("Error :" + errors);
                }
                if (await _roleManager.RoleExistsAsync(UserRole.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRole.User);
                }
                return "Success!";
            }
            catch (Exception ex)
            {
                return "Failed to create user" + ex;
            }
        }

        //[Authorize(Roles = UserRole.Admin)]
        [AllowAnonymous]
        public async Task<string> RegisterAdmin(RegisterUser register)
        {
            try
            {
                var isUserExist = await _userManager.FindByNameAsync(register.UserName);
                if (isUserExist != null)
                {
                    return "User already exists";
                }
                var user = new ApplicationIdentityUser
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = register.UserName,
                    Email = "",
                };
                var result = await _userManager.CreateAsync(user, register.Password);
                if (!result.Succeeded)
                {
                    return "Error: Cannot create Admin username/password .. please try again";
                }
                if (!await _roleManager.RoleExistsAsync(UserRole.Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
                }
                if (!await _roleManager.RoleExistsAsync(UserRole.User))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.User));
                }
                if (!await _roleManager.RoleExistsAsync(UserRole.Editor))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.Editor));
                }
                if (await _roleManager.RoleExistsAsync(UserRole.Admin))
                {
                    await _userManager.AddToRoleAsync(user, UserRole.Admin);
                }
                return "Success!";
            }
            catch (Exception ex)
            {
                return "Failed to create user! error: " + ex;
            }
        }

        [AllowAnonymous]
        public async Task<UserLognDto> LoginAccount(RegisterUser login)
        {
            try
            {
                var loginUser = await _userManager.FindByNameAsync(login.UserName);
                if (
                    loginUser != null
                    && await _userManager.CheckPasswordAsync(loginUser, login.Password)
                )
                {
                    var generatedToken = new SymmetricSecurityKey(
                        await _userManager.CreateSecurityTokenAsync(loginUser)
                    );
                    var authSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!)
                    );
                    var role = "";
                    var userRole = await _userManager.GetRolesAsync(loginUser);

                    foreach (var userrole in userRole)
                    {
                        role = userrole + ",";
                    }

                    await _userManager.RemoveAuthenticationTokenAsync(
                        loginUser,
                        "userIdentity",
                        role
                    );
                    var newRefreshToken = await _userManager.GenerateUserTokenAsync(
                        loginUser,
                        "userIdentity",
                        role
                    );
                    await _userManager.SetAuthenticationTokenAsync(
                        loginUser,
                        "userIdentity",
                        role,
                        newRefreshToken
                    );

                    var authClaim = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginUser.Id),
                        new Claim(ClaimTypes.GivenName, ""),
                        new Claim(ClaimTypes.Name, loginUser.UserName!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    foreach (var userrole in userRole)
                    {
                        authClaim.Add(new Claim(ClaimTypes.Role, userrole));
                    }

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(8),
                        claims: authClaim,
                        signingCredentials: new SigningCredentials(
                            authSigningKey,
                            SecurityAlgorithms.HmacSha256
                        )
                    );

                    var userInfo = new UserLognDto
                    {
                        UserId = loginUser.Id,
                        NewRefreshToken = newRefreshToken,
                        UserRole = userRole.ToList(),
                        UserName = loginUser.UserName!,
                        UserToken = new JwtSecurityTokenHandler().WriteToken(token)
                    };
                    return userInfo;
                }
                return new UserLognDto();
            }
            catch (Exception ex)
            {
                return new UserLognDto();
            }
        }
    }
}
