using System.Security.Claims;
using API.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(SignInManager<ApplicationUser> _signInManager) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto) 
        {
            var user = new ApplicationUser 
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _signInManager.UserManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> logout() 
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated;
            if (isAuthenticated == false) return NoContent();

            var user = await _signInManager.UserManager.Users
                    .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            if (user == null) return Unauthorized();

            return Ok(new 
            {
                user.FirstName,
                user.LastName,
                user.Email
            });
        }

        [HttpGet]
        public ActionResult GetAuthState()
        {
            return Ok(new 
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }

    }
}