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

        [HttpPost("create-workout")]
        [Authorize]
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

        [HttpGet("this-weeks-workouts/overview")]
        [Authorize]
        public async Task<ActionResult<List<AssignedWorkout>>> GetThisWeeksWorkouts()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var db = _contextFactory.CreateDbContext();

                if (db == null)
                {
                    return StatusCode(500, "Database connection failed");
                }

                //Get dates for this week only
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-1 * (int)today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);

                //Get all workouts for this week
                //Apply filter for user
                //var assignedWorkouts = db.AssignedWorkouts.Where(n => n.AssigneeId == user.Id);

                //Apply filter for this week
                //assignedWorkouts = assignedWorkouts.Where(n => n.WorkoutDate >= startOfWeek && n.WorkoutDate <= endOfWeek);

                //Include workout info
                //assignedWorkouts = assignedWorkouts.Include(n => n.Workout);

                List<AssignedWorkout> assignedWorkouts = new List<AssignedWorkout>();

                //ensure count
                if (assignedWorkouts.Count() == 0)
                {
                    //TEMP CODE
                    //Create fake workouts that fall within the week
                    //This is for testing purposes only

                    var workout1 = new Workout
                    {
                        WorkoutName = "Workout 1",
                        Notes = "This is a note for workout 1",
                        CoachId = user.Id
                    };

                    var workout2 = new Workout
                    {
                        WorkoutName = "Workout 2",
                        Notes = "This is a note for workout 2",
                        CoachId = user.Id
                    };

                    var workout3 = new Workout
                    {
                        WorkoutName = "Workout 3",
                        Notes = "This is a note for workout 3",
                        CoachId = user.Id
                    };

                    var workout4 = new Workout
                    {
                        WorkoutName = "Workout 4",
                        Notes = "This is a note for workout 4",
                        CoachId = user.Id
                    };

                    var workout5 = new Workout
                    {
                        WorkoutName = "Workout 5",
                        Notes = "This is a note for workout 5",
                        CoachId = user.Id
                    };

                    //Ad workouts to assigned workouts set the dates to this week
                    var assignedWorkout1 = new AssignedWorkout
                    {
                        Workout = workout1,
                        Index = 1,
                        CoachName = "Sarah",
                        WorkoutDate = startOfWeek.AddDays(1),
                        WorkoutName = "Workout 1",
                        Notes = "This is a note for workout 1",
                        AssigneeId = user.Id
                    };

                    var assignedWorkout2 = new AssignedWorkout
                    {
                        Workout = workout2,
                        Index = 2,
                        CoachName = "Sarah",
                        WorkoutDate = startOfWeek.AddDays(2),
                        WorkoutName = "Workout 2",
                        Notes = "This is a note for workout 2",
                        AssigneeId = user.Id
                    };

                    var assignedWorkout3 = new AssignedWorkout
                    {
                        Workout = workout3,
                        Index = 3,
                        CoachName = "Sarah",
                        WorkoutDate = startOfWeek.AddDays(3),
                        WorkoutName = "Workout 3",
                        Notes = "This is a note for workout 3",
                        AssigneeId = user.Id
                    };

                    var assignedWorkout4 = new AssignedWorkout
                    {
                        Workout = workout4,
                        Index = 4,
                        CoachName = "Sarah",
                        WorkoutDate = startOfWeek.AddDays(4),
                        WorkoutName = "Workout 4",
                        Notes = "This is a note for workout 4",
                        AssigneeId = user.Id
                    };

                    var assignedWorkout5 = new AssignedWorkout
                    {
                        Workout = workout5,
                        Index = 5,
                        CoachName = "Sarah",
                        WorkoutDate = startOfWeek.AddDays(5),
                        WorkoutName = "Workout 5",
                        Notes = "This is a note for workout 5",
                        AssigneeId = user.Id
                    };

                    Console.WriteLine("Adding fake workouts to list");

                    //return fake workouts
                    return Ok(new List<AssignedWorkout> { assignedWorkout1, assignedWorkout2, assignedWorkout3, assignedWorkout4, assignedWorkout5 });

                    return NotFound("No workouts found for this week");
                }

                return Ok(assignedWorkouts.ToList());

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("workouts-by-date-range/{startDate}/{endDate}")]
        [Authorize]
        public async Task<ActionResult<List<Workout>>> GetWorkoutsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {

                using var db = _contextFactory.CreateDbContext();

                Console.WriteLine("Getting workouts by date range");

                //First ensure user is logged in
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return BadRequest("User not found");
                }

                //Get Workouts in the date range under the user id
                //From assigned workouts grab the workout ids that fall within the date range
                //Then grab the workouts that have those ids
                var assignedWorkouts = await db.AssignedWorkouts.Where(n => n.AssigneeId == user.Id && n.WorkoutDate >= startDate.Date && n.WorkoutDate.Date <= endDate.Date).ToListAsync();              //if no workouts found return empty list
                List<int> workoutIds = new List<int>();
                if (assignedWorkouts.Count() == 0)
                {
                    if (assignedWorkouts.Count() == 0)
                    {
                        //var response = new { Message = "No workouts found for this date range" };
                        //return Ok(response);

                        //TEST CODE
                        workoutIds = new List<int> { 6 };
                    }

                }
                else
                {
                    workoutIds = assignedWorkouts.Select(n => n.WorkoutId).ToList();
                }


                //Grab the workouts that have the ids
                var workouts = db.Workouts.Where(n => workoutIds.Contains(n.WorkoutId));

                //if no workouts found return empty list
                if (workouts.Count() == 0)
                {
                    var response = new { Message = "No workouts found for this date range" };
                    return Ok(response);
                }

                //else grab workout blocks under the ids
                var workoutBlocks = db.WorkoutBlocks.Where(n => workoutIds.Contains(n.WorkoutId)).Include(n => n.Parameters);

                //if no workout blocks found return empty list
                if (workoutBlocks.Count() == 0)
                {
                    var response = new { Message = "No workout blocks found for this date range" };
                    return Ok(response);
                }

                //Attach workout blocks to workouts
                foreach (var workout in workouts)
                {
                    //Set workout blocks
                    workout.WorkoutBlocks = workoutBlocks.Where(n => n.WorkoutId == workout.WorkoutId).ToList();

                    //Set date
                    //workout.DateToComplete = assignedWorkouts.Where(n => n.WorkoutId == workout.WorkoutId).Select(p => p.WorkoutDate).FirstOrDefault();
                    //TEST CODE
                    workout.DateToComplete = DateTime.Now;

                    //set coach name
                    string coachName = db.UserInformation
                        .Where(n => n.UserId.Equals(workout.CoachId))
                        .Select(p => p.FirstName + " " + p.LastName)
                        .FirstOrDefault() ?? "404";

                    workout.CoachName = coachName;
                }

                var workoutsList = await workouts.ToListAsync();

                await db.DisposeAsync();

                Console.WriteLine("Good Return");
                //return workouts
                return Ok(workoutsList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // [HttpPost("generate-test-workout")]
        // [Authorize]
        // public async Task<ActionResult> GenerateTestWorkout()
        // {

        // }

    }



}
