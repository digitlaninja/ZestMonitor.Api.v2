using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Services;

namespace ZestMonitor.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public AuthService AuthService { get; }
        public AuthController(AuthService authService)
        {
            AuthService = authService;
        }

        [HttpPost("register", Name = "register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            user.Username = user.Username.ToLower();

            if (await this.AuthService.UserExists(user.Username))
                return Conflict(new { Error = "Please choose another Username, this one already exists." });

            var userRegistered = await this.AuthService.Register(user);
            if (!userRegistered)
                return StatusCode((int)HttpStatusCode.ServiceUnavailable);

            var body = new { Data = user.Username };
            // return CreatedAtRoute(new Uri($"{Request.Path}", UriKind.Relative), body);

            return CreatedAtRoute("register", body);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

                var validUser = await this.AuthService.Login(model);

                // dont show user not found or any messages with hints, simply unath
                if(validUser == null)
                    return Unauthorized();

                var accessToken = this.AuthService.CreateJwtAccessToken(validUser);
                return Ok(new {token = accessToken});
        }
    }
}