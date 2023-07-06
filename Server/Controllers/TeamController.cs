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
        public async Task<ActionResult> SubmitCoachRegistration([FromBody] CoachRegistration coachRegistration)
        {
            try
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    IdentityUser user = await _userManager.GetUserAsync(User);

                    if (user == null)
                    {
                        return StatusCode(500, "User was null");
                    }

                    //update user email if it is different
                    if (coachRegistration.EmailIsCorrect == false)
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
                        OwnerID = user.Id,
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

                    //Update user information table so the coach now points towards the team
                    var userInformation = await db.UserInformation.FirstOrDefaultAsync(n => n.UserId == user.Id);
                    userInformation.TeamID = team.TeamID;
                    userInformation.City = coachRegistration.TeamLocationCity;
                    userInformation.State = coachRegistration.TeamLocationState;

                    await db.SaveChangesAsync();

                    return Ok();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("team-athletes/{teamID}")]
        [Authorize]
        public async Task<ActionResult<List<UserInformation>>> GetTeamsAthletes(int teamID)
        {
            try
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    //Verify that the team exists
                    var team = await db.Teams.Where(n => n.TeamID == teamID).FirstOrDefaultAsync();
                    if (team == null)
                    {
                        //TODO: Log this
                        //Team does not exist
                        return StatusCode(404, "Team does not exist");
                    }
                    else
                    {
                        //Get user information for all athletes on the team
                        IdentityUser user = await _userManager.GetUserAsync(User);
                        if(user != null)
                        {
                            List<UserInformation> athleteInformation = await db.UserInformation.Where(n => n.TeamID == teamID && !n.UserId.Equals(user.Id)).ToListAsync();
                            return Ok(athleteInformation);
                        }
                        else
                        {
                            List<UserInformation> athleteInformation = await db.UserInformation.Where(n => n.TeamID == teamID).ToListAsync();
                            return Ok(athleteInformation);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("team/include-children/{teamID}")]
        [Authorize]
        public async Task<ActionResult<Team>> GetUsersTeam(int teamID)
        {
            try
            {
                using(var db = _contextFactory.CreateDbContext())
                {
                    //Get Team and include children
                    var team = await db.Teams.Where(n => n.TeamID == teamID)
                        .Include(n => n.TeamInfo)
                        .Include(n => n.TeamPackage)
                        .FirstOrDefaultAsync();

                    if(team != null)
                    {
                        return Ok(team);
                    }
                    else
                    {
                        return StatusCode(404, "Team does not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

