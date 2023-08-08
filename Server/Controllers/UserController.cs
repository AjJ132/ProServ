using System;
using ProServ.Shared.Models.NET_CORE_USER;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProServ.Shared.Models.UserInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProServ.Server.Contexts;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProServ.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IDbContextFactory<ProServDbContext> _contextFactory;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(IDbContextFactory<ProServDbContext> contextFactory, UserManager<IdentityUser> userManager)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
    }


    //----------------------------------------------------------------------------------------

    //Check if user profile exists
    [HttpGet("profile/exists")]
    [Authorize]
    public async Task<ActionResult<bool>> CheckIfProfileExists()
    {

        try
        {
            var db = _contextFactory.CreateDbContext();
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User was null");
            }

            var userProfile = await db.UserProfile.Where(n => n.UserId.Equals(user.Id)).FirstOrDefaultAsync();
            if (userProfile != null)
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Debug.WriteLine("Error 1004: UserProfile");
            Console.WriteLine("Error 1004: UserProfile");
            return BadRequest("Error 1004: UserProfile");
        }
    }


    // GET: User profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfile>> GetUserProfile()
    {

        try
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var userProfile = await db.UserProfile.FindAsync(user.Id);

                    if (userProfile == null)
                    {
                        return NotFound();
                    }

                    return Ok(userProfile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Debug.WriteLine("Error 1001: UserProfile");
                Console.WriteLine("Error 1001: UserProfile");
                return BadRequest("Error 1001: UserProfile");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            Debug.WriteLine("Error 1004: UserProfile");
            Console.WriteLine("Error 1004: UserProfile");
            return BadRequest("Error 1004: UserProfile");
        }


    }

    // PUT: User profile
    [HttpPut("Profile")]
    [Authorize]
    public async Task<IActionResult> PutUserProfile(UserProfile userProfile)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string userId = currentUser.Id;

        if (userId != userProfile.UserId)
        {
            return BadRequest();
        }

        using (var db = _contextFactory.CreateDbContext())
        {
            db.Entry(userProfile).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }

    // POST user profile
    [HttpPost("Profile")]
    [Authorize]
    public async Task<IActionResult> PostUserProfile([FromBody] UserProfile userProfile)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            try
            {
                await db.UserProfile.AddAsync(userProfile);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest("Error 1002: UserProfile");
            }

            return Ok("User Profile created successfully");
        }
    }

    //---------------------------------------------------------------------------------------

    //Check if user profile exists
    [HttpGet("user-information/exists")]
    [Authorize]
    public async Task<ActionResult<bool>> CheckIfUserInfoExists()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var userInformation = await db.UserInformation.FindAsync(user.Id);
                    if (userInformation != null)
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Debug.WriteLine("Error 1001: UserInformation");
                Console.WriteLine("Error 1001: UserInformation");
                return BadRequest("Error 1001: UserProfile");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Debug.WriteLine("Error 1004: UserProfile");
            Console.WriteLine("Error 1004: UserProfile");
            return BadRequest("Error 1004: UserProfile");
        }

    }

    [HttpGet("user-information")]
    [Authorize]
    public async Task<ActionResult<UserInformation>> GetUserInformation()
    {
        try
        {
            //Temporary for IOS TESTING ## TODO REMOVE ME
            //var user = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByEmailAsync("aj132@icloud.com");
            try
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var userInformation = await db.UserInformation.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));

                    if (userInformation != null)
                    {
                        return Ok(userInformation);
                    }
                    else
                    {
                        return NotFound("User information not found");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Debug.WriteLine("Error 1001: UserInformation");
                Console.WriteLine("Error 1001: UserInformation");
                return BadRequest("Error 1001: UserInformation");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Debug.WriteLine("Error 1004: UserProfile");
            Console.WriteLine("Error 1004: UserProfile");
            return BadRequest("Error 1004: UserProfile");
        }
    }

    // PUT User Information
    [HttpPut("UserInformation")]
    [Authorize]
    public async Task<IActionResult> PutUserInformation(UserInformation userInformation)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        if (id != userInformation.UserId)
        {
            return BadRequest();
        }

        using (var db = _contextFactory.CreateDbContext())
        {
            db.Entry(userInformation).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }

    // Post User Information
    [HttpPost("UserInformation")]
    [Authorize]
    public async Task<ActionResult> UserInformation(UserInformation userInformation)
    {
        using (var db = _contextFactory.CreateDbContext())
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest("Error 1001: UserInformation: User is null");
                }
                userInformation.UserId = user.Id;
                db.UserInformation.Add(userInformation);
                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return BadRequest("Error 1002: UserInformation");
            }
        }
    }

    [HttpGet("UserInformation/UserId")]
    [Authorize]
    public async Task<ActionResult<int>> GetUserId()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            string id = user.Id;

            if (id == null)
            {
                return NotFound("User ID was null");
            }

            return Ok(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return StatusCode(500, "Something went wrong while getting user id");
        }
    }


}

