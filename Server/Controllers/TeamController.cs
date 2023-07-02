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
    public class TeamController : ControllerBase
    {
        private readonly IDbContextFactory<ProServDbContext> _contextFactory;
        private readonly UserManager<IdentityUser> _userManager;

        public TeamController(IDbContextFactory<ProServDbContext> contextFactory, UserManager<IdentityUser> userManager)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
        }

        [HttpGet("all-packages")]
        [Authorize]
        public async Task<ActionResult<List<AllTeamPackages>>> GetAllTeamPackagesAsync()
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                try
                {
                    var teamPackages = await db.AllTeamPackages.Where(n => n.IsPublic).ToListAsync();
                    if (teamPackages == null || !teamPackages.Any())
                    {
                        return NotFound("No public team packages found.");
                    }

                    return Ok(teamPackages);
                }
                catch (Exception ex)
                {
                    // Log error here, for example, using NLog, Log4Net, or any other logging framework
                    // logger.Error(ex, "An error occurred while getting all team packages.");
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            
        }
        [HttpGet("team-name-exists/{teamName}")]
        [Authorize]
        public async Task<ActionResult<bool>> TeamNameExistsAsync(string teamName)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                try
                {
                    var team = await db.Teams.FirstOrDefaultAsync(n => n.TeamName.Equals(teamName));
                    if (team == null)
                    {
                        return Ok(false);
                    }

                    return Ok(true);
                }
                catch (Exception ex)
                {
                    //TODO: Research loggers
                    // Log error here, for example, using NLog, Log4Net, or any other logging framework
                    // logger.Error(ex, "An error occurred while getting all team packages.");
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            
        }

        [HttpPost("coach-registration")]
        [Authorize]
        public async Task<ActionResult> SubmitCoachRegistration([FromBody]CoachRegistration coachRegistration)
        {
            try
            {
                

                using(var db = _contextFactory.CreateDbContext())
                {
                    IdentityUser user = await _userManager.GetUserAsync(User);

                    if(user == null)
                    {
                        return StatusCode(500, "User was null");
                    }

                    //Get users current roles so we can remove them
                    var roles = await _userManager.GetRolesAsync(user);
                    if(roles != null && roles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, roles);
                    }

                    //Add coach role
                    await _userManager.AddToRoleAsync(user, "Coach");

                    //update user email if it is different
                    if(coachRegistration.EmailIsCorrect == false)
                    {
                        user.Email = coachRegistration.Email;
                        await _userManager.UpdateAsync(user);
                        //TODO add error handling //Maybe report this to some kind of log for SUDO admin to see
                    }

                    //TODO store user address somewhere

                    var team = new Team
                    {
                        TeamName = coachRegistration.TeamName,
                        Location = coachRegistration.TeamLocationCity + ", " + coachRegistration.TeamLocationState,
                        Terminated = false,
                    };

                    await db.Teams.AddAsync(team);
                    await db.SaveChangesAsync();

                    var teamInfo = new TeamInfo
                    {
                        TeamID = team.TeamID,
                        DateCreated = DateTime.Now,
                        OwnerID = coachRegistration.UserID,
                        IsSchoolOrganization = coachRegistration.IsSchoolOrganization,
                        TeamPackageID = coachRegistration.PackageID,
                        TimeChanged = 0,
                        TeamSport = coachRegistration.TeamSportSpecify,
                    };

                    var teamPackage = new TeamPackage
                    {
                        TeamID = team.TeamID,
                        PackageID = coachRegistration.PackageID,
                        PackageStart = DateTime.Now,
                        PackageEnd = DateTime.Now.AddDays(30),
                    };

                    await db.TeamInfo.AddAsync(teamInfo);
                    await db.TeamPackage.AddAsync(teamPackage);

                    await db.SaveChangesAsync();

                    return Ok();

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}

