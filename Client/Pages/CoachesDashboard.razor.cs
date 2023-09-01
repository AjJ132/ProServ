using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ProServ.Client;
using ProServ.Client.Shared;
using Radzen;
using Radzen.Blazor;
using ProServ.Client.Data;
using ProServ.Shared.Models.Coaches;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ProServ.Client.Controllers;
using ProServ.Shared.Models.UserInfo;

using System.Net.Http.Headers;
using System.Diagnostics;
using ProServ.Shared.Models.Workouts;
using ProServ.Shared.Models.Util;
using Newtonsoft.Json.Bson;


namespace ProServ.Client.Pages
{
    public partial class CoachesDashboard : IDisposable
    {
        private bool _isLoading = true;

        RadzenScheduler<AssignedWorkout> _calendar;
        private IEnumerable<AssignedWorkout> _assignedWorkouts;
        private List<AssignedWorkout> _workoutsForDay;
        private DateTime _selectedDate = DateTime.Now;


        private IEnumerable<UserInformation> _myAtheletes;
        private bool _loadingMyteam = true;
        private bool _userHasNoTeam = false;



        private UserInformation _myInformation;
        protected override async Task OnInitializedAsync()
        {
            //Get users information
            try
            {
                var userInfoResponse = await Http.GetAsync("api/User/user-information");
                if (userInfoResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success getting user ifnormation");
                    var info = await userInfoResponse.Content.ReadFromJsonAsync<UserInformation>();
                    Console.WriteLine("Content was read from json");
                    if (info != null)
                    {
                        if (info.UserId != null)
                        {
                            Console.WriteLine("User id is not null");
                            _myInformation = info;
                            info = null;
                            userInfoResponse = null;
                        }
                    }
                }
                else
                {
                    NavigationManager.NavigateTo("/");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
            }

            //Load the athletes on my team
            Console.WriteLine("Loading my team");
            await LoadMyTeam();
            Console.WriteLine("My team was loaded");
            Console.WriteLine("Loading calendar data");
            await LoadCalendarData();
            Console.WriteLine("Calendar data was loaded");

            _isLoading = false;
            //Get Coach's assigned workouts    
            await base.OnInitializedAsync();
        }

        private async Task LoadMyTeam()
        {
            if (_myInformation.TeamID != 0)
            {
                try
                {
                    var athleteInformationResponse = await Http.GetAsync($"api/Team/team-athletes/{_myInformation.TeamID}");

                    if (athleteInformationResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Success getting athletes");

                        var athleteWrapper = await athleteInformationResponse.Content.ReadFromJsonAsync<ResponseWrapper<UserInformation>>();

                        if (athleteWrapper?.values != null && athleteWrapper.values.Count > 0)
                        {
                            Console.WriteLine("Athlete Wrapper Count: " + athleteWrapper.values.Count);
                            _myAtheletes = athleteWrapper.values;
                        }
                        else
                        {
                            Console.WriteLine("No athletes found");
                            _myAtheletes = new List<UserInformation>();
                        }
                    }
                    else
                    {
                        _myAtheletes = new List<UserInformation>();
                        Console.WriteLine("API call unsuccessful");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during deserialization: " + ex.Message);
                }


            }
            else
            {
                _userHasNoTeam = true;
            }
            _loadingMyteam = false;
        }

        private async Task LoadCalendarData()
        {
            //First load all the workouts for the month
            var assignedWorkoutsResponse = await Http.GetAsync($"api/workout/my-team-workouts-by-month?todaysDate={DateTime.Now}");


            //if response is successful
            if (assignedWorkoutsResponse.IsSuccessStatusCode)
            {
                var wrapper = await assignedWorkoutsResponse.Content.ReadFromJsonAsync<ResponseWrapper<AssignedWorkout>>();
                var assignedWorkouts = wrapper.values ?? new List<AssignedWorkout>();
                if (assignedWorkouts.Count() > 0)
                {
                    _assignedWorkouts = assignedWorkouts;

                    //Get the workouts for the selected day
                    _workoutsForDay = _assignedWorkouts.Where(x => x.WorkoutDate.Date == _selectedDate.Date).ToList();
                }
                else
                {
                    _assignedWorkouts = new List<AssignedWorkout>();
                    _workoutsForDay = new List<AssignedWorkout>();
                }
            }
            else
            {
                _assignedWorkouts = new List<AssignedWorkout>();
            }
            //Use Load data event based on start date and end date. 
            //Display workouts for each day
            //In athletes container display workouts for each athlete for that day.
            //if no workout display no workout message
        }

        //Assign User Workout
        private async void AssignUserWorkout (UserInformation user)
        {
            //Convert user information to short_user
            var selectedUser = new User_Short
            {
                id = user.UserId,
                name = user.FirstName + " " + user.LastName
            };

            await localStorageService.SetItemAsync("selectedUser", selectedUser);

            //Navigate to assign workout page
            NavigationManager.NavigateTo("/workout-center");
        }

        //Calendar Methods
        private async Task OnDateSelect(DateTime date)
        {
            _selectedDate = date;
            Console.WriteLine("Selected date: " + _selectedDate);
            _workoutsForDay = _assignedWorkouts.Where(x => x.WorkoutDate.Date == _selectedDate.Date).ToList();
            await InvokeAsync(StateHasChanged);
        }

        void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
            {
                args.Attributes["style"] = "background: rgba(255,220,40,.2);";
            }

            if (args.View.Text == "Month" && args.Start.Date == _selectedDate.Date)
            {
                args.Attributes["style"] = "background: rgba(40,220,40,.2);";

            }
        }

        async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {
            //set selected date
            await OnDateSelect(args.Start);
            await InvokeAsync(StateHasChanged);
        }
        async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<AssignedWorkout> args)
        {

        }

        void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<AssignedWorkout> args)
        {

        }

        public void Dispose()
        {
            //TODO Determine if this is the best way to manage memory
            GC.Collect();
        }
    }

}