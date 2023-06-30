using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProServ.Shared.Models;
using ProServ.Shared.Models.NET_CORE_USER;
using ProServ.Shared.Models.UserInfo;
using Microsoft.IdentityModel.Tokens;
using ProServ.Server.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ProServ.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IDbContextFactory<ProServDbContext> _contextFactory;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config, IDbContextFactory<ProServDbContext> contextFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _contextFactory = contextFactory;
        }

        //Register new user
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUser model)
        {
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email + "-newUser"

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");

                //Ensure that the user is authenticated and log in
                await _signInManager.SignInAsync(user, isPersistent: false);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // Add this line
                };


                // Fetch these values from the configuration instead of hardcoding
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var issuer = _config["Jwt:Issuer"];
                var audience = _config["Jwt:Audience"];

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), userId = user.Id });


            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(RegisterUser model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid email or password.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);


            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // Add this line
                };

                //fetch user roles
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }



                // Fetch these values from the configuration instead of hardcoding
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var issuer = _config["Jwt:Issuer"];
                var audience = _config["Jwt:Audience"];

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            else
            {
                return BadRequest("Invalid email or password.");
            }
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpGet("SUID")]
        [Authorize]
        public async Task<ActionResult<string>> GetLoggedInUserID()
        {
            //Get the current user id
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            return Ok(user.Id);
        }



        //Get user email
        [HttpGet("Email")]
        [Authorize]
        public async Task<ActionResult<string>> GetUserEmailAsync()
        {
            try
            {
                //Get the current user id
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                return Ok(user.Email);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in finding the user in which to get the email");
                return BadRequest("Couldnt find the specified user");
            }
        }

        //Update IdentityUser PhoneNumber
        [HttpPut("PhoneNumber")]
        [Authorize]
        public async Task<IActionResult> UpdateIdentityUserPhoneNumber([FromBody] string phoneNumber)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                user.PhoneNumber = phoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("Failed to update the users phone number");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in finding the user in which to update the phone number");
                return BadRequest("Couldnt find the specified user");
            }
        }

        //Update IdentityUser Username
        [HttpPut("Username")]
        [Authorize]
        public async Task<IActionResult> UpdateIdentityUserUsername(string username)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            string id = currentUser.Id;

            var user = await _userManager.FindByIdAsync(id);
            user.UserName = username;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

