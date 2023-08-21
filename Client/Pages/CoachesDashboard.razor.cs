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
using ProServ.Shared.Models.Workouts;

namespace ProServ.Client.Pages
{
    public partial class CoachesDashboard : IDisposable
    {
        private bool _isLoading = true;

        RadzenScheduler<AssignedWorkout> _calendar;
        private IEnumerable<AssignedWorkout> _assignedWorkouts;


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

            _isLoading = false;
            //Get Coach's assigned workouts    
            await base.OnInitializedAsync();
        }


        private async Task LoadMyTeam()
        {
            if (_myInformation.TeamID != 0)
            {
                var ahtleteInformationResponse = await Http.GetAsync($"api/Team/team-athletes/{_myInformation.TeamID}");
                if (ahtleteInformationResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success getting athletes");
                    var athleteWrapper = await ahtleteInformationResponse.Content.ReadFromJsonAsync<AthleteWrapper>();
                    var athletes = athleteWrapper?.Values ?? new List<UserInformation>();
                    Console.WriteLine("Atheletes: " + athletes.Count());
                    if (athletes.Count() > 0)
                    {
                        _myAtheletes = athletes;
                        athletes = null;
                        ahtleteInformationResponse = null;
                    }
                    else
                    {
                        _myAtheletes = new List<UserInformation>();
                        athletes = null;
                        ahtleteInformationResponse = null;
                    }
                }
                else
                {
                    _myAtheletes = new List<UserInformation>();
                }
            }
            else
            {
                _userHasNoTeam = true;
            }
            _loadingMyteam = false;
        }

        void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {

        }

        async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {

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