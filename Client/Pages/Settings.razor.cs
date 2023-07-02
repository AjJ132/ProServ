using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ProServ.Shared;
using ProServ.Shared.Models.Coaches;
using ProServ.Shared.Models.UserInfo;

namespace ProServ.Client.Pages;

public partial class Settings : ComponentBase
{

    [Inject]
    public HttpClient Http { get; set; }
    [Inject]
    public AuthenticationStateProvider AuthProvider { get; set; }
    [Inject]
    public NavigationManager NavManager { get; set; }

    //Authentication state variables


    //All user variables
    private UserInformation _userInformation;
    private UserProfile _userProfile;
    UserGoals _userGoals;
    ReportedInjuries _userInjuries;
    UserCoachingStyle _userCoachingStyle;
    UserTrackRecords _userTrackRecords;
    private string _userPhoneNumber = "706-436-1212";
    private string _userEmail = "aj132@icloud.com";
    private string _password = "Password Here";

    //UserSubscription _userSubscription;
    private string _userRole = "Member";
    private bool _isOnTeam = false;
    private bool _isCoach = false;

    //All coach variables
    private Team _team;
    private TeamInfo _teamInfo;
    private TeamPackage _teamPackage;

    //bool to make user settings not displayed
    private bool _missingUserInformation = false;
    private bool _missingUserProfile = false;
    private bool _missingUserRecords = false;

    //regex
    private string _emailRegex = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$\n";

    bool _isLoaded = false;


    protected override async Task OnInitializedAsync()
    {
        //Fetch user data from API
        try
        {
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            string userID = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userInfoRequest = await Http.GetAsync($"api/User/user-information");
            if (userInfoRequest.IsSuccessStatusCode)
            {
                this._userInformation = await userInfoRequest.Content.ReadFromJsonAsync<UserInformation>();




                //Check user info to see if they are apart of a team
                if (_userInformation.TeamID != 0 | _userInformation.TeamID != null)
                {
                    this._isOnTeam = true;

                    //TODO fetch team information
                }

            }
            else
            {
                this._missingUserInformation = true;
            }

            var userProfileRequest = await Http.GetAsync($"api/User/profile");
            if (userProfileRequest.IsSuccessStatusCode)
            {
                this._userProfile = await userProfileRequest.Content.ReadFromJsonAsync<UserProfile>();
            }
            else
            {
                this._missingUserProfile = true;
            }


            this._userTrackRecords = new UserTrackRecords();
            //var userTrackRecordsRequest = await Http.GetAsync($"api/User/track-records");
            //if(userTrackRecordsRequest.IsSuccessStatusCode)
            //{
            //    //this._userTrackRecords = await userTrackRecordsRequest.Content.ReadFromJsonAsync<UserTrackRecords>();

            //    this._userTrackRecords = new UserTrackRecords();
            //}
            //else
            //{
            //    this._missingUserRecords = true;
            //}

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


        _isLoaded = true;
    }

    private async Task SaveProfileChanges(UserInformation newUserInfo)
    {

    }

    private void ProfileChangesInvalid()
    {
        Console.WriteLine("The changes you made to your profile are invalid.");
    }

    private void SaveTrackRecordChanges(UserTrackRecords records)
    {

    }

    //all coaches and team
    private async Task SaveTeamChanges(Team team)
    {

    }

    private async Task SaveTeamInfoChanges(TeamInfo teamInfo)
    {

    }

    private async Task SaveTeamPackageChanges(TeamPackage teamPackage)
    {

    }

    private void StartCoachesRegistration()
    {
        NavManager.NavigateTo("/Coaches-Registration");
    }

    private List<string> _states = new List<string>
    {
        "Alabama",
        "Alaska",
        "Arizona",
        "Arkansas",
        "California",
        "Colorado",
        "Connecticut",
        "Delaware",
        "Florida",
        "Georgia",
        "Hawaii",
        "Idaho",
        "Illinois",
        "Indiana",
        "Iowa",
        "Kansas",
        "Kentucky",
        "Louisiana",
        "Maine",
        "Maryland",
        "Massachusetts",
        "Michigan",
        "Minnesota",
        "Mississippi",
        "Missouri",
        "Montana",
        "Nebraska",
        "Nevada",
        "New Hampshire",
        "New Jersey",
        "New Mexico",
        "New York",
        "North Carolina",
        "North Dakota",
        "Ohio",
        "Oklahoma",
        "Oregon",
        "Pennsylvania",
        "Rhode Island",
        "South Carolina",
        "South Dakota",
        "Tennessee",
        "Texas",
        "Utah",
        "Vermont",
        "Virginia",
        "Washington",
        "West Virginia",
        "Wisconsin",
        "Wyoming"
    };
}
