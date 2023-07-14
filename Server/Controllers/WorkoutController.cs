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
using ProServ.Shared.Models.Coaches;
using System.Diagnostics;
using System.Globalization;
using ProServ.Shared.Models.Workouts;

namespace ProServ.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly IDbContextFactory<ProServDbContext> _contextFactory;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkoutController(IDbContextFactory<ProServDbContext> contextFactory, UserManager<IdentityUser> userManager)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
        }

        [HttpGet("test")]
        [Authorize]
        public async Task<ActionResult> TestConnection()
        {
            string s = "";
            return Ok();
        }

        [HttpPost("create-workout")]
        //[Authorize]
        public async Task<ActionResult> AddNewWorkout(Workout newWorkout)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (user.Id != null)
                {
                    newWorkout.CoachId = user.Id;
                }
                else
                {
                    return NotFound("User ID not found");
                }

                var db = _contextFactory.CreateDbContext();
                if (db == null)
                {
                    return StatusCode(500, "Database connection failed");
                }

                await db.Workouts.AddAsync(newWorkout);
                await db.SaveChangesAsync();


                return Ok("New Workout Created");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
