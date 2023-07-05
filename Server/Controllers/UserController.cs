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



    [HttpGet("user-role")]
    [Authorize] // Ensures that the user is authenticated
    public async Task<ActionResult<string>> GetUserRole()
    {
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user);

        // You may have more logic here to determine what to return if the user has multiple roles

        return roles.ToList().FirstOrDefault();// Return the first role, or whatever your logic determines
    }


    //----------------------------------------------------------------------------------------

    //Check if user profile exists
    [HttpGet("profile/exists")]
    [Authorize]
    public async Task<ActionResult<bool>> CheckIfProfileExists()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var userProfile = await db.UserProfile.FindAsync(user.Id);
                    if (userProfile != null)
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
                Debug.WriteLine("Error 1001: UserProfile");
                Console.WriteLine("Error 1001: UserProfile");
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
            var user = await _userManager.GetUserAsync(User);

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
                if(user == null)
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
    public async Task<IActionResult> GetUserId()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        return Ok(id);
    }


    //----------------------------------------------------------------------------------------
    // Get UserTrackRecords
    [HttpGet("track-records")]
    [Authorize]
    public async Task<ActionResult<UserTrackRecords>> GetUserTrackRecords()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var userTrackRecords = await db.UserTrackRecords.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id));
                    return Ok(userTrackRecords);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Debug.WriteLine("Error 1001: UserTrackRecords");
                Console.WriteLine("Error 1001: UserTrackRecords");
                return BadRequest("Error 1001: UserTrackRecords");
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine("Error: " + ex.Message);
            Debug.WriteLine("Error 1004: UserTrackRecords");
            Console.WriteLine("Error 1004: UserTrackRecords");
            return BadRequest("Error 10014 UserTrackRecords");
        }
    }

    // PUT UserTrackRecords
    [HttpPut("Trackrecords")]
    [Authorize]
    public async Task<IActionResult> PutUserTrackRecords(UserTrackRecords userTrackRecords)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string userId = currentUser.Id;

        if (userId != userTrackRecords.UserId)
        {
            return BadRequest();
        }

        using (var db = _contextFactory.CreateDbContext())
        {
            db.Entry(userTrackRecords).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Error updating user track records");
                throw;
            }

            return NoContent();
        }
    }

    // POST UserTrackRecords
    [HttpPost("track-records")]
    [Authorize]
    public async Task<ActionResult> PostUserTrackRecords(UserTrackRecords userTrackRecords)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            userTrackRecords.UserId = user.Id;

            using (var db = _contextFactory.CreateDbContext())
            {
                db.UserTrackRecords.Add(userTrackRecords);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    Debug.WriteLine("Error 1001: UserTrackRecords");
                    Console.WriteLine("Error 1001: UserTrackRecords");
                    return BadRequest("Error 1001: UserTrackRecords");
                }

                return CreatedAtAction("GetUserTrackRecords", new { id = userTrackRecords.UserId }, userTrackRecords);
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine("Error: " + ex.Message);
            Debug.WriteLine("Error 1002: UserTrackRecords");
            Console.WriteLine("Error 1002: UserTrackRecords");
            return BadRequest("Error 1002 UserTrackRecords");
        }
    }



    //----------------------------------------------------------------------------------------

    // Get Profile Onboarding
    [HttpGet("Onboarding")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ProfileOnboarding>>> GetUserProfileOnboarding()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.ProfileOnboarding.Where(x => x.UserId.Equals(id)).ToListAsync();
        }
    }

    // PUT Profile Onboarding
    [HttpPut("Onboarding")]
    [Authorize]
    public async Task<IActionResult> PutProfileOnboarding(ProfileOnboarding profileOnboarding)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        if (id != profileOnboarding.UserId)
        {
            return BadRequest();
        }

        using (var db = _contextFactory.CreateDbContext())
        {
            db.Entry(profileOnboarding).State = EntityState.Modified;

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

    // Post Profile Onboarding
    [HttpPost("Onboarding")]
    [Authorize]
    public async Task<IActionResult> PostProfileOnboarding(ProfileOnboarding profileOnboarding)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        profileOnboarding.UserId = currentUser.Id;

        using (var db = _contextFactory.CreateDbContext())
        {
            db.ProfileOnboarding.Add(profileOnboarding);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtAction("GetProfileOnboarding", new { id = profileOnboarding.UserId }, profileOnboarding);
        }
    }

    //See if user completed onboarding
    [HttpGet("Onboarding/Completed")]
    [Authorize]
    public async Task<ActionResult<bool>> GetProfileOnboardingCompleted()
    {

        using (var db = _contextFactory.CreateDbContext())
        {
            var user = await _userManager.GetUserAsync(User);

            //get user ProfileOnboarding
            ProfileOnboarding profileOnboarding = await db.ProfileOnboarding.FindAsync(user.Id);

            if (profileOnboarding == null)
            {
                //if its null then check if the user can be found in the system,

                //Create user onboarding table in database with the specified ID
                ProfileOnboarding newOnboarding = new ProfileOnboarding() { UserId = user.Id, Completed = false };

                //Add to the database
                await db.ProfileOnboarding.AddAsync(newOnboarding);
                await db.SaveChangesAsync();

                return Ok(false);
            }
            else
            {
                if (profileOnboarding.Completed)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
        }
    }

    //Mark user as completed onboarding
    [HttpPut("onboarding/complete")]
    [Authorize]
    public async Task<ActionResult> CompleteProfileOnboarding([FromBody] bool status)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            using (var db = _contextFactory.CreateDbContext())
            {
                try
                {
                    ProfileOnboarding profileOnboarding = await db.ProfileOnboarding.FindAsync(user.Id);
                    if (profileOnboarding == null)
                    {
                        return BadRequest("Error 1003: ProfileOnboarding");
                    }

                    profileOnboarding.Completed = status;
                    await db.SaveChangesAsync();

                    return Ok("Profile Onboarding marked complete");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);
                    return BadRequest("Error 1003: ProfileOnboarding");
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
            return BadRequest("Error 1003: ProfileOnboarding");
        }
    }


    //----------------------------------------------------------------------------------------

    // Get Reported Injuries
    [HttpGet("Injuries")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ReportedInjuries>>> GetUserReportedInjuries()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.ReportedInjuries.Where(x => x.UserId.Equals(id)).ToListAsync();
        }
    }

    // PUT Reported Injuries
    [HttpPut("Injuries")]
    [Authorize]
    public async Task<IActionResult> PutReportedInjuries(ReportedInjuries reportedInjuries)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        if (id != reportedInjuries.UserId)
        {
            return BadRequest();
        }

        using (var db = _contextFactory.CreateDbContext())
        {
            db.Entry(reportedInjuries).State = EntityState.Modified;

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

    // Post Reported Injuries
    [HttpPost("Injuries")]
    [Authorize]
    public async Task<ActionResult<ReportedInjuries>> PostReportedInjuries(ReportedInjuries reportedInjuries)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        reportedInjuries.UserId = currentUser.Id;

        using (var db = _contextFactory.CreateDbContext())
        {
            db.ReportedInjuries.Add(reportedInjuries);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtAction("GetReportedInjuries", new { id = reportedInjuries.UserId }, reportedInjuries);
        }
    }

    //----------------------------------------------------------------------------------------

    // Get User Goals
    [HttpGet("Goals")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserGoals>>> GetUserGoals()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        using (var db = _contextFactory.CreateDbContext())
        {
            return await db.UserGoals.Where(x => x.UserId.Equals(id)).ToListAsync();
        }
    }

    // PUT User Goals
    [HttpPut("Goals")]
    [Authorize]
    public async Task<IActionResult> PutUserGoals(UserGoals userGoals)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        string id = currentUser.Id;

        if (id != userGoals.UserId)
        {
            return BadRequest();
        }

        using (var db = _contextFactory.CreateDbContext())
        {
            db.Entry(userGoals).State = EntityState.Modified;

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

    // Post User Goals
    [HttpPost("Goals")]
    [Authorize]
    public async Task<ActionResult<UserGoals>> PostUserGoals(UserGoals userGoals)
    {
        var user = await _userManager.GetUserAsync(User);
        userGoals.UserId = user.Id;

        using (var db = _contextFactory.CreateDbContext())
        {
            db.UserGoals.Add(userGoals);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtAction("GetUserGoals", new { id = userGoals.UserId }, userGoals);
        }
    }

    //----------------------------------------------------------------------------------------






    //----------------------------------------------------------------------------------------

}

