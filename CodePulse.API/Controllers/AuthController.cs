using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository token)
        {
            this.userManager = userManager;
            this.tokenRepository = token;
        }

        //POST: {apibaseurl}/api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            //Create IdentityUser object

            var user = new IdentityUser
            {
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim(),
            };

            //Create User
            var identityResult = await userManager.CreateAsync(user, request.Password);
            if (identityResult.Succeeded)
            {
                // Add Role to user reader
                identityResult = await userManager.AddToRoleAsync(user, "Reader");
                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }

        //POST: {apibaseurl}/api/auth/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request) 
        {
            // Check email
            var identityUser = await userManager.FindByNameAsync(request.Email);
            bool isUserValid = false;
            if (identityUser is not null) 
            {
                //Check password
                isUserValid = await userManager.CheckPasswordAsync(identityUser, request.Password);
                
                if (isUserValid) 
                {
                    var roles = userManager.GetRolesAsync(identityUser);

                    //Create Token and response
                    var response = new LoginResponseDto
                    {
                        Email = request.Email,
                        Roles = roles.Result.ToList(),
                        Token = tokenRepository.CreateJwtToken(identityUser, roles.Result.ToList())
                    };

                    return Ok(response);
                }
            }
            ModelState.AddModelError("", "Email or Password incorrenct");
            return ValidationProblem(ModelState);
        }
    }
}
