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

namespace ProServ.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoachController : ControllerBase
    {
        private readonly IDbContextFactory<ProServDbContext> _contextFactory;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CoachController(IDbContextFactory<ProServDbContext> contextFactory, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private async Task<bool> VerifyCoach(IdentityUser user)
        {
            var role = await _userManager.GetRolesAsync(user);
            if(!role.FirstOrDefault().Equals("Coach"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [HttpGet("assigned-workouts")]
        [Authorize]
        public async Task<ActionResult<List<AllTeamPackages>>> GetMyAssignedWorkouts()
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if(user == null)
                    {
                        return NotFound("User not found.");
                    }

                    bool IsCoach = await VerifyCoach(user);
                    if(IsCoach)
                    {
                        var assignedWorkouts = await db.AssignedWorkouts.Where(n => n.AssigneeId.Equals(user.Id)).ToListAsync();
                        if (assignedWorkouts == null || !assignedWorkouts.Any())
                        {
                            return NotFound("No public team packages found.");
                        }

                        return Ok(assignedWorkouts);
                    }
                    else
                    {
                        return Unauthorized("User is not a coach.");
                    }

                }
                catch (Exception ex)
                {
                    // Log error here, for example, using NLog, Log4Net, or any other logging framework
                    // logger.Error(ex, "An error occurred while getting all team packages.");
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            
        }
        
        

    }
}

