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
            var workout = this.NewWorkout;

            //Send workout over HTTP

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
            this._selectedBlock.Parameters.Add(new Parameter() { BlockType = updatedType.ToString() });

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


    }
}

