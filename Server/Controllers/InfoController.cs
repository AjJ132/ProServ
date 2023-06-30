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

namespace ProServ.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly IDbContextFactory<ProServDbContext> _contextFactory;

        public InfoController(IDbContextFactory<ProServDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
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
        [HttpGet("team-name-exists")]
        [Authorize]
        public async Task<ActionResult<bool>> TeamNameExistsAsync([FromBody]string teamName)
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

    }
}

