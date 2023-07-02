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
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace ProServ.Client.Pages
{
    public partial class Coaches Dashboard
    {
        //New stuff
        private UserInformation _userInformation;
        //Check it page is loading
        private bool _isLoading = true;
        //user permission state
        private string _userRole = "Member";
        protected override async Task OnInitializedAsync()
        {
            //Get user permission state
            try
            {
                var response = await Http.GetAsync("api/User/user-role");
                if (response.IsSuccessStatusCode)
                {
                    this._userRole = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                Console.WriteLine("Error : " + ex);
            }

            await base.OnInitializedAsync();
        }

        //New stuff
        //Old Stuff
        private double _currentMileage = 14;
        private double _goalMileage = 40;
        RadzenScheduler<WorkoutDate> _scheduler;
        IList<WorkoutDate> workoutDates = new List<WorkoutDate>
        {
            new WorkoutDate
            {
                Start = DateTime.Today.AddDays(-2),
                End = DateTime.Today.AddDays(-2),
                Text = "Birthday"
            },
            new WorkoutDate
            {
                Start = DateTime.Today.AddDays(-1),
                End = DateTime.Today.AddDays(-1),
                Text = "Meeting"
            },
            new WorkoutDate
            {
                Start = DateTime.Today,
                End = DateTime.Today,
                Text = "Lunch"
            },
            new WorkoutDate
            {
                Start = DateTime.Today.AddDays(1),
                End = DateTime.Today.AddDays(1),
                Text = "Meeting"
            },
            new WorkoutDate
            {
                Start = DateTime.Today.AddDays(2),
                End = DateTime.Today.AddDays(2),
                Text = "Birthday"
            },
        };
        class Workout
        {
            public string status { get; set; }
            public double percent { get; set; }
        }

        class WorkoutDate
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string Text { get; set; }
        }

        Workout[] workouts = new Workout[]
        {
            new Workout()
            {
                status = "Good",
                percent = 40
            },
            new Workout()
            {
                status = "Okay",
                percent = 20
            },
            new Workout()
            {
                status = "Bad",
                percent = 10
            },
            new Workout()
            {
                status = "Not Complete",
                percent = 30
            },
        };
        void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            // Highlight today in month view
            if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
            {
                args.Attributes["style"] = "background: rgba(255,220,40,.2);";
            }

            // Highlight working hours (9-18)
            if ((args.View.Text == "Week" || args.View.Text == "Day") && args.Start.Hour > 8 && args.Start.Hour < 19)
            {
                args.Attributes["style"] = "background: rgba(255,220,40,.2);";
            }
        }
    }
}