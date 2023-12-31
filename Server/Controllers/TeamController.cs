﻿using System;
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
            var db = _contextFactory.CreateDbContext();

            try
            {
                if (string.IsNullOrEmpty(teamName))
                {
                    return BadRequest("Team name cannot be null or empty.");
                }

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

        [HttpGet("team")]
        [Authorize]
        public async Task<ActionResult<Team>> GetTeam()
        {
            var db = _contextFactory.CreateDbContext();

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var teamID = await db.UserInformation.Where(n => n.UserId.Equals(user.Id)).Select(p => p.TeamID).FirstOrDefaultAsync();

                if (teamID == 0)
                {
                    return BadRequest("User is not apart of a team");
                }

                var team = await db.Teams.FirstOrDefaultAsync(n => n.TeamID == teamID);

                //pre convert team to json and print to console
                var teamJson = Newtonsoft.Json.JsonConvert.SerializeObject(team);
                Console.WriteLine(teamJson);

                if (team == null)
                {
                    return BadRequest("Team was not found");
                }

                return Ok(team);
            }
            catch (Exception ex)
            {
                //TODO: Research loggers
                // Log error here, for example, using NLog, Log4Net, or any other logging framework
                // logger.Error(ex, "An error occurred while getting all team packages.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }


        [HttpGet("team/name/{id}")]
        [Authorize]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var db = _contextFactory.CreateDbContext();

            try
            {
                if (id == 0)
                {
                    return BadRequest("Team ID cannot be 0");
                }

                var team = await db.Teams.Where(n => n.TeamID == id).FirstOrDefaultAsync();

                if (team == null)
                {
                    return StatusCode(500, "Team was null");
                }

                return Ok(team);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

                    team.TeamPackage = teamPackage;
                    team.TeamInfo = teamInfo;

                    await db.Teams.AddAsync(team);
                    await db.SaveChangesAsync();
                    //await db.TeamInfo.AddAsync(teamInfo);
                    //await db.TeamPackage.AddAsync(teamPackage);

                    //Update user information table so the coach now points towards the team
                    var userInformation = await db.UserInformation.Where(n => n.UserId.Equals(user.Id)).FirstOrDefaultAsync();
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
        public async Task<ActionResult<List<UserInformation>>> GetTeamAthletes(int teamID)
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
                        if (user != null)
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
        public async Task<ActionResult<Team>> GetUsersTeamAndIncludeChildren(int teamID)
        {
            try
            {
                var db = _contextFactory.CreateDbContext();

                //Get Team and include children
                var team = await db.Teams.Where(n => n.TeamID == teamID)
                    .Include(n => n.TeamInfo)
                    .Include(n => n.TeamPackage)
                    .FirstOrDefaultAsync();

                if (team != null)
                {
                    return Ok(team);
                }
                else
                {
                    return NotFound("Team does not exist");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("team/athletes/all/{teamID}")]
        [Authorize]
        public async Task<ActionResult<List<UserInformation>>> GetAllAthletesByTeamIdAsync(int teamID)
        {
            try
            {
                //Never will have 50,000 teams so dont need to check for that and waste a db call
                if (teamID < 0 || teamID > 50000)
                {
                    return BadRequest("Invalid team ID");
                }

                var db = _contextFactory.CreateDbContext();
                var athletes = await db.UserInformation.Where(n => n.TeamID == teamID).ToListAsync();

                if (athletes == null)
                {
                    return BadRequest("No users were found");
                }

                if (athletes.Count() == 0)
                {
                    return NotFound("No users were found");
                }
                else
                {
                    return Ok(athletes);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("team/athletes/data/{teamID}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserInformation>>> GetAthletesDataByTeamIdAndCount(int teamID, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                var db = _contextFactory.CreateDbContext();
                return Ok(await db.UserInformation.Where(n => n.TeamID == teamID)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpGet("team/athletes/count/{teamID}")]
        [Authorize]
        public async Task<ActionResult<int>> GetAthletesCount(int teamId)
        {
            try
            {
                var db = _contextFactory.CreateDbContext();
                return Ok(await db.UserInformation
                    .Where(n => n.TeamID == teamId)
                    .CountAsync());
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

