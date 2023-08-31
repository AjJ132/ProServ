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

        protected override async Task OnInitializedAsync()
        {
            //Init Workout
            NewWorkout = new Workout();
            NewWorkout.WorkoutBlocks = new List<WorkoutBlock>();
            NewWorkout.WorkoutBlocks.Add(new WorkoutBlock() { BlockName = _blockTypes[0], BlockType = _blockTypes[0] });
        }


        //Create and Save the new workout
        private async Task CreateWorkout()
        {
            Workout workout = this.NewWorkout;

            var authState = await AuthProvider.GetAuthenticationStateAsync();
            string userID = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            workout.CoachId = userID;




            //Send workout over HTTP
            var createWorkoutResponse = await Http.PostAsJsonAsync("api/Workout/create-workout", workout);
            if (createWorkoutResponse.IsSuccessStatusCode)
            {
                //TODO: Reset interface
                Console.WriteLine("Workout Created");
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
            _selectedBlockOrder = blockOrder;

            if (this._selectedBlock == null)
            {
                this._selectedBlock = new WorkoutBlock();
            }

            this._selectedBlock = this.NewWorkout.WorkoutBlocks.Where(x => x.BlockOrder == blockOrder).FirstOrDefault();
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
            //this._selectedAthlete = (User_Short)user;
            var user2 = (User_Short)user;
            Console.WriteLine("Selected Athlete: " + user2.name);

            _selectedAthlete = null;
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

