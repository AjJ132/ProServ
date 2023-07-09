using System.Net.Http.Json;
using ProServ.Shared.Models.UserInfo;
using Radzen;

namespace ProServ.Client.Pages
{
    public partial class TeamCenter
    {
        private bool _isLoading = true;
        private bool _loadingTable = true;

        private List<UserInformation> _myAthletesTest = new List<UserInformation>();
        private int _myAthletesCountTest = 0;
        private List<UserInformation> _myAssistantCoachesTest = new List<UserInformation>();
        
        protected override async Task OnInitializedAsync()
        {
            //First check if the user has a team registered
            //Get User Information

            //Generate some fake data
           
            createTestData();   
           
        
           


            this._isLoading = false;
            await base.OnInitializedAsync();
        }

        public void createTestData()
        {

             _myAthletesTest.Add(new UserInformation()
            {
                UserId = "one",
                FirstName = "John",
                LastName = "Doe",
                TeamID = 10,
                City = "Columbus",
                State = "Ohio",
                Height = "6'2",
                Weight = 200,
                IsInHighschool = true,
                School = "Columbus High School",
                Birthday = DateTime.Now.AddYears(-16),
                Gender = "Male",
                ActiveUser = true,
            });
            _myAthletesTest.Add(new UserInformation()
            {
                UserId = "two",
                FirstName = "Jane",
                LastName = "Smith",
                TeamID = 20,
                City = "Cleveland",
                State = "Ohio",
                Height = "5'8",
                Weight = 160,
                IsInHighschool = false,
                School = "Cleveland University",
                Birthday = DateTime.Now.AddYears(-22),
                Gender = "Female",
                ActiveUser = true,
            });

            _myAthletesTest.Add(new UserInformation()
            {
                UserId = "three",
                FirstName = "James",
                LastName = "Brown",
                TeamID = 30,
                City = "Dayton",
                State = "Ohio",
                Height = "6'0",
                Weight = 185,
                IsInHighschool = true,
                School = "Dayton High School",
                Birthday = DateTime.Now.AddYears(-17),
                Gender = "Male",
                ActiveUser = true,
            });

            _myAthletesTest.Add(new UserInformation()
            {
                UserId = "four",
                FirstName = "Emily",
                LastName = "Johnson",
                TeamID = 40,
                City = "Cincinnati",
                State = "Ohio",
                Height = "5'6",
                Weight = 150,
                IsInHighschool = false,
                School = "Cincinnati University",
                Birthday = DateTime.Now.AddYears(-24),
                Gender = "Female",
                ActiveUser = false,
            });

            _myAssistantCoachesTest.Add(new UserInformation()
            {
                UserId = "five",
                FirstName = "Robert",
                LastName = "Williams",
                TeamID = 50,
                City = "Toledo",
                State = "Ohio",
                Height = "6'1",
                Weight = 190,
                IsInHighschool = false,
                School = "Toledo University",
                Birthday = DateTime.Now.AddYears(-30),
                Gender = "Male",
                ActiveUser = true,
            });

            _myAssistantCoachesTest.Add(new UserInformation()
            {
                UserId = "six",
                FirstName = "Elizabeth",
                LastName = "Taylor",
                TeamID = 60,
                City = "Akron",
                State = "Ohio",
                Height = "5'7",
                Weight = 160,
                IsInHighschool = false,
                School = "Akron University",
                Birthday = DateTime.Now.AddYears(-35),
                Gender = "Female",
                ActiveUser = true,
            });

            
        }

        private async Task LoadAthletes(int teamID, LoadDataArgs args)
        {
            this._loadingTable = true;

            try
            {
                
                int pageSize = args.Top.Value;
                int pageIndex = args.Skip.Value / pageSize;

                this._myAthletesTest = await Http.GetFromJsonAsync<List<UserInformation>>($"api/team/athletes/data/{teamID}?pageIndex={pageIndex}&pageSize={pageSize}");
                this._myAthletesCountTest = await Http.GetFromJsonAsync<int>($"api/team/athletes/count/{teamID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _myAthletesTest = new List<UserInformation>();
                _myAthletesCountTest = 0;
            }

            this._loadingTable = false;
        }

    }
}