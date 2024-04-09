using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController(IUserAccount accountInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(RegisterDto user)
        {
            if(user == null)
            {
                return BadRequest("Register data is Empty");
            }
            
            var result = await accountInterface.CreateAsync(user);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(LoginDto user)
        {
            if (user == null)
            {
                return BadRequest("Login Data is Empty");
            }

            var result = await accountInterface.SignInAsync(user);
            return Ok(result);
        }
        [HttpPut("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto token)
        {
            if (token == null)
                return BadRequest("Token data is empty");

            var result = await accountInterface.RefreshTokenAsync(token);
            return Ok(result);
        }
    }
}
