using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace API.Controllers
{
    public class AccountController(SignInManager<User> signInManager) : BaseAPIController
    {

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegisterDto dto)
        {
            var user = new User
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.Email
            };
            var result = await signInManager.UserManager.CreateAsync(user, dto.Password);
            if(result.Succeeded)
                return Ok(new { message = "User registered successfully" });
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem();
        }

        [AllowAnonymous]
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false)
                return NoContent();
            var user = await signInManager.UserManager.GetUserAsync(User);
            if(user == null)
                return Unauthorized();
            return Ok(new
            {
                user.DisplayName,
                user.Email,
                user.UserName,
                user.Bio,
                user.ImageUrl
            });
        }



        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { message = "User logged out successfully" });
        }
    }
}
