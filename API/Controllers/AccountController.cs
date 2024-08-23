using System.Security.Claims;
using API.Dtos;
using API.Extensions;
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

            if (!result.Succeeded) 
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

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

            var user = await _signInManager.UserManager.GetUserByEmailWithAddress(User);

            return Ok(new 
            {
                user.FirstName,
                user.LastName,
                user.Email,
                Address = user.Address?.ToDto()
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

        [Authorize]
        [HttpPost("address")] 
        public async Task<ActionResult<Address>> CreateOrUpdateAddress(AddressDto dto)
        {
            var user = await _signInManager.UserManager.GetUserByEmailWithAddress(User);

            if (user.Address is null)
            {
                user.Address = dto.ToEntity();
            }
            else
            {
                user.Address.UpdateFromDto(dto);
            }

            var result = await _signInManager.UserManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Problem when updating user address!");

            return Ok(user.Address.ToDto());
        }

    }
}