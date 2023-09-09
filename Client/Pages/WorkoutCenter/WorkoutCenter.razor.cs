using System;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ProServ.Shared;
using ProServ.Shared.Models.Coaches;
using ProServ.Shared.Models.UserInfo;
using ProServ.Shared.Models.Workouts;

using Microsoft.AspNetCore.Components;
using ProServ.Client.Data;
using Radzen;
using ProServ.Shared.Models.Util;
using Radzen.Blazor;
using Blazored.LocalStorage;

namespace ProServ.Client.Pages.WorkoutCenter
{

    public partial class WorkoutCenter
    {

        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthProvider { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public ILocalStorageService LocalStorage { get; set; }
        private List<UserInformation> _workoutAssignees = new List<UserInformation>();
        private IEnumerable<User_Short> _myAtheletes;
        private int _myAtheletesCount = 0;
        private User_Short _selectedAthlete;


        private List<string> _blockTypes = new List<string>() { "Warmup", "Strength", "Long Run", "Cooldown", "Custom" };
        private string _selectedBlockTypes = "Warmup";
        public Workout NewWorkout { get; set; }
        private int? _selectedBlockOrder = null;
        private WorkoutBlock _selectedBlock;
        private Parameter _selectedBlockParameter;
        private string _coachName = "";
        private bool _hasAssignees = false;
        private DateTime _dateToComplete = DateTime.Today;

        protected override async Task OnInitializedAsync()
        {
            //Init Workout
            NewWorkout = new Workout();
            NewWorkout.WorkoutName = "New Workout";
            NewWorkout.WorkoutBlocks = new List<WorkoutBlock>();
            NewWorkout.WorkoutBlocks.Add(new WorkoutBlock() { BlockName = _blockTypes[0], BlockType = _blockTypes[0] });

            //Check to see if there is a user_short in local storage. If there is grab it and set the selected athelte and then delete it from local storage
            var selectedUser = await LocalStorage.GetItemAsync<User_Short>("selectedUser");

            if (selectedUser != null)
            {
                _selectedAthlete = selectedUser;
                await LocalStorage.RemoveItemAsync("selectedUser");
            }
        }


        //Create and Save the new workout
        private async Task CreateWorkout()
        {
            Workout workout = this.NewWorkout;

            //Get the current user's ID from Claims
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var userID = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var coachName = authState.User.FindFirst(ClaimTypes.Name).Value;

            //Assign Coach ID from Claims
            workout.CoachId = userID;
            //Assign Coach Name from Claims
            workout.CoachName = coachName;

            //Determine if athlete is selected
            if (_selectedAthlete != null)
            {
                AssignedWorkout newAssign = new()
                {
                    AssigneeId = _selectedAthlete.id,
                    CoachId = userID,
                    CoachName = coachName,
                    WorkoutDate = _dateToComplete,
                    WorkoutName = workout.WorkoutName,
                    ReportBack = false, //TODO: Add report back
                    Notes = "", //TODO: Add notes
                    WorkoutId = 0, //TEMP. ID will be added after workout is created
                    Workout = workout,
                };

                //send assigned workout over HTTP
                var assignWorkoutResponse = await Http.PostAsJsonAsync("api/Workout/create-assign-workout", newAssign);
                if (assignWorkoutResponse.IsSuccessStatusCode)
                {
                    //Reset interface
                    //TODO add success message/notification
                }
                else
                {
                    //TODO: Add error message
                }
            }
            else
            {
                //Workout has no assignee, add it to database. TODO: Add message to user to assign or add to collection. 
                //TODO add collection feature
                //Send workout over HTTP
                var createWorkoutResponse = await Http.PostAsJsonAsync("api/Workout/create-workout", workout);
                if (createWorkoutResponse.IsSuccessStatusCode)
                {
                    //TODO: Reset interface
                    //TODO: Add success message
                    Console.WriteLine("Workout Created");
                }
            }


        }
        private void AddNewWorkoutBlock()
        {
            WorkoutBlock newBlock = new WorkoutBlock()
            {
                BlockOrder = NewWorkout.WorkoutBlocks.Count + 1,
                BlockName = _blockTypes[0],
                BlockType = _blockTypes[0]

            };
            this.NewWorkout.WorkoutBlocks.Add(newBlock);
            StateHasChanged();
        }

        void SelectBlock(int blockOrder)
        {
            if (_selectedBlockOrder == blockOrder)
            {
                // Toggle off the selected block
                _selectedBlockOrder = null;
                _selectedBlock = null;
            }
            else
            {
                _selectedBlockOrder = blockOrder;

                if (_selectedBlock == null)
                {
                    _selectedBlock = new WorkoutBlock();
                }

                _selectedBlock = NewWorkout.WorkoutBlocks.Where(x => x.BlockOrder == blockOrder).FirstOrDefault();
            }
        }

        void BlockTypeDropDownChange(object updatedType)
        {
            this._selectedBlock.BlockType = updatedType.ToString();
            this._selectedBlock.BlockName = updatedType.ToString();

            this.NewWorkout.WorkoutBlocks.Where(x => x.BlockOrder == _selectedBlockOrder).FirstOrDefault().BlockType = updatedType.ToString();
            this.NewWorkout.WorkoutBlocks.Where(x => x.BlockOrder == _selectedBlockOrder).FirstOrDefault().BlockName = updatedType.ToString();

            //Set the new parameter
            if (this._selectedBlock.Parameters == null)
            {
                this._selectedBlock.Parameters = new List<Parameter>();
            }

            this._selectedBlock.Parameters.Clear();
            this._selectedBlock.Parameters.Add(new Parameter());

        }
        private void UpdateLongRunData(LongRunData data)
        {
            //Grab the first parameter from the selected block
            //## TODO NEED TO FIX THIS
            Parameter parameter = this._selectedBlock.Parameters.FirstOrDefault();
            if (parameter != null) // Ensure the parameter is not null
            {
                parameter.sDistance1 = data.Distance;

                // If it's a pace then combine the seconds and minutes into a single timespan
                if (data.PaceOrTotal)
                {
                    var pace = new TimeSpan(0, data.PaceMinutes, data.PaceSeconds);
                    parameter.TTime1 = pace;
                }
                else // else set the time to the total pace minutes
                {
                    parameter.TTime1 = new TimeSpan(0, data.PaceMinutes, 0);
                }
            }
        }
        private void UpdateCustomData(CustomData data)
        {
            //Grab the first parameter from the selected block
            //## TODO NEED TO FIX THIS
            Parameter parameter = this._selectedBlock.Parameters.FirstOrDefault();
            if (parameter != null) // Ensure the parameter is not null
            {
                parameter.SValue1 = data.Comments;
                parameter.TTime1 = data.Time;
                parameter.sDistance1 = data.Distance;
            }
        }

        async void MyTeamLoadDataEvent(LoadDataArgs args)
        {
            string searchFilter = args.Filter.ToLower();

            //check if the search filter is empty
            if (string.IsNullOrEmpty(searchFilter))
            {
                Console.WriteLine("Search filter is empty; fetching all athletes.");
            }


            var encodedSearchFilter = Uri.EscapeDataString(searchFilter);
            var athleteResponse = await Http.GetAsync($"api/workout/search-athletes?searchFilter={encodedSearchFilter}");


            if (athleteResponse.IsSuccessStatusCode)
            {
                var wrapper = await athleteResponse.Content.ReadFromJsonAsync<ResponseWrapper<User_Short>>();
                _myAtheletes = wrapper.values ?? new List<User_Short>();
                _myAtheletesCount = _myAtheletes.Count();
                //var k = _myAtheletes.FirstOrDefault().Item1;
            }
            else
            {
                _myAtheletes = new List<User_Short>();
                _myAtheletesCount = 0;
            }


            await InvokeAsync(StateHasChanged);
        }

        async void SelectAthlete(object user)
        {
            //Convert user to User_Short
            var userShort = (User_Short)user;

            //check if the user is null 
            if (userShort == null)
            {
                Console.WriteLine("User is null");
                _selectedAthlete = null;
                _hasAssignees = false;
                return;
            }

            Console.WriteLine("Selected Athlete: " + userShort.name);

            _selectedAthlete = userShort;
            _hasAssignees = true;
            await InvokeAsync(StateHasChanged);

        }

        private Timer debounceTimer;
        private void Debounce(Action method, int milliseconds = 300)
        {
            debounceTimer?.Dispose();
            debounceTimer = new Timer(_ => method(), null, milliseconds, Timeout.Infinite);
        }

    }
}

