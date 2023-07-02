
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Radzen.Blazor;
using ProServ.Shared.Models.UserInfo;
using ProServ.Shared.Models.Coaches;
using System.Diagnostics;
using System.Security.Claims;

namespace ProServ.Client.Pages.Login_and_Onboarding
{
    public partial class CoachesRegistration
    {
        private RadzenTemplateForm<CoachRegistration> _genInfoform;
        private string _errorMessageGenInfo;
        private bool _genInfoHasErrors = false;
        private RadzenTemplateForm<CoachRegistration> _teamInfoForm;
        private string _errorMessageTeamInfo;
        private bool _teamInfoHasErrors = false;
        private AllTeamPackages _selectedPackage;
        //Coaches Registration Data
        private CoachRegistration _coachRegistration = new CoachRegistration();
        private bool _createTeamNow = false;
        //bool to check if new email is needed
        private bool _doesntNeedNewEmail = true;
        //Afiliated with a school
        private bool _isAffiliatedWithSchool = false;
        private bool _otherSportSelected = false;
        //false means monthly true means yearly
        private bool _monthlyOrYearly = false;
        protected override async Task OnInitializedAsync()
        {
            //Get user permission state
            try
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity.IsAuthenticated)
                {
                    //await GetUserInformation();
                    this._coachRegistration.UserID = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    this._coachRegistration.TeamLocationState = this._states[0];
                    //Get user email
                    this._coachRegistration.EmailIsCorrect = true;
                    var emailRequest = await Http.GetAsync($"api/Auth/Email");
                    if (emailRequest.IsSuccessStatusCode)
                    {
                        var email = await emailRequest.Content.ReadAsStringAsync();
                        this._coachRegistration.Email = email;
                    }
                    else
                    {
                        this._coachRegistration.EmailIsCorrect = false;
                    }
                }
                else
                {
                    //Return user to homepage if they are unable to be authenticated
                    Console.WriteLine("There was an error authenticating the user");
                    NavigationManager.NavigateTo("/Dashboard");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                Console.WriteLine("Error : " + ex);
                NavigationManager.NavigateTo("/Dashboard");
            }

            await base.OnInitializedAsync();
        }

        //change CSS rules for selecting a package
        void SelectPackage(AllTeamPackages package)
        {
            if (_selectedPackage == package)
            {
                _selectedPackage = null; // Deselect the package if it's already selected
            }
            else
            {
                _selectedPackage = package; // Select the package
            }
        }

        void YearlMonthlySwitchChange(bool value)
        {
            StateHasChanged();
        }

        //pull user information
        private async Task GetUserInformation()
        {
            var userInfoResponse = await Http.GetAsync($"api/User/user-information");
            if (userInfoResponse.IsSuccessStatusCode)
            {
                try
                {
                    //## NEED TO IMPLEMENT PROPER ERROR HANDLING
                    UserInformation userInfo = await userInfoResponse.Content.ReadFromJsonAsync<UserInformation>();
                    this._coachRegistration = new CoachRegistration();
                    var userEmailResponse = await Http.GetAsync($"api/Auth/email");
                    if (userEmailResponse.IsSuccessStatusCode)
                    {
                        try
                        {
                            string userEmail = await userEmailResponse.Content.ReadAsStringAsync();
                            this._coachRegistration.Email = userEmail;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error: " + ex);
                            Console.WriteLine("Error : " + ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex);
                    Console.WriteLine("Error : " + ex);
                    NavigationManager.NavigateTo("/Settings");
                }
            }
        }

        private void CreateTeamLater()
        {
            if (_createTeamNow)
            {
                _createTeamNow = false;
                StateHasChanged();
            }
            else
            {
                _createTeamNow = true;
                StateHasChanged();
            }
        }

        private void OnInvalidSubmitTeamInfoForm()
        {
        //TODO: Add invalid submit to form
        }

        private async Task SubmitRegistration()
        {
            try
            {
                //validate that team name has not already been taken
                try
                {
                    //validate that team name has not already been taken
                    var teamNameResponse = await Http.GetAsync($"api/Team/team-name-exists/{_coachRegistration.TeamName}");
                    if (teamNameResponse.IsSuccessStatusCode)
                    {
                        bool teamNameTaken = await teamNameResponse.Content.ReadFromJsonAsync<bool>();
                        if (teamNameTaken)
                        {
                            //TODO: implement error message to form
                            Debug.WriteLine("Team name already taken");
                            Console.WriteLine("Team name taken");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex);
                    Console.WriteLine("Error : " + ex);
                    throw;
                }

                //Coaches Registration Data
                try
                {
                    //Get teams selected package
                    if (this._selectedPackage != null)
                    {
                        this._coachRegistration.PackageID = this._selectedPackage.PackageID;
                    }
                    else
                    {
                        Console.WriteLine("Selected a package first");
                        return;
                    }

                    //set team school
                    if (!this._coachRegistration.AffliatedSchool.Equals(""))
                    {
                        this._coachRegistration.IsSchoolOrganization = true;
                    }
                    else
                    {
                        this._coachRegistration.IsSchoolOrganization = false;
                    }

                    //Get teams sport
                    if (this._coachRegistration.TeamSportSpecify.Equals(""))
                    {
                        this._coachRegistration.TeamSportSpecify = this._coachRegistration.TeamSport;
                    }

                    var coachRegistrationResponse = await Http.PostAsJsonAsync<CoachRegistration>("api/Team/coach-registration", _coachRegistration);
                    if (coachRegistrationResponse.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Coach Registration Successful");
                        NavigationManager.NavigateTo("/Coaches-Dashboard");
                    //TODO Send email to new coach and when navigate allow for tutorial
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex);
                    Console.WriteLine("Error : " + ex);
                    throw;
                }
            }
            catch (Exception e)
            {
            }
        }

        //TODO REMOVE THIS
        private void InfoToConsole()
        {
            //Console.Writeline evreything
            Console.WriteLine("Team Name: " + _coachRegistration.TeamName);
            Console.WriteLine("Team Location: " + _coachRegistration.TeamLocationCity + ", " + _coachRegistration.TeamLocationState);
            Console.WriteLine("Team Sport: " + _coachRegistration.TeamSport);
        }

        private void CancelRegistration()
        {
            //Navigate back to previous page
            NavigationManager.NavigateTo("/Settings");
        }

        private void TeamSportDropDownChange(string value)
        {
            if (value.Equals("Other"))
            {
                this._otherSportSelected = true;
                StateHasChanged();
            }
            else
            {
                this._coachRegistration.TeamSportSpecify = "";
                this._otherSportSelected = false;
                StateHasChanged();
            }
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
        private List<string> _sports = new List<string>
        {
            "Cross Country",
            "Marathons",
            "Track and Field",
            "Triathlon",
            "General Fitness",
            "Other"
        };
        private List<AllTeamPackages> _allPackages = new List<AllTeamPackages>
        {
            new AllTeamPackages()
            {
                PackageID = 4,
                PackageName = "Trial Package",
                PackageDescription = "Trial package includes basic features that last up to 14 days after the coaches registration.",
                PackageSubtext = "Free Trial",
                PackagePriceMonthly = new decimal (0),
                PackagePriceYearly = new decimal (0),
                PackageMaxMembers = 3,
                PackageMaxAssistantCoaches = 0,
                PackageFeatures = new List<string>
                {
                    "Basic workout builder functionality",
                    "Access to nutrition services",
                    "Basic synced services",
                    "14 Day Trial"
                },
                IsPublic = true
            },
            new AllTeamPackages()
            {
                PackageID = 2,
                PackageName = "Starter Package",
                PackageDescription = "For small teams and great for coaches starting their buisness. Does allow a trial!",
                PackageSubtext = "Begginer Friendly",
                PackagePriceMonthly = new decimal (15.0),
                PackagePriceYearly = new decimal (120.0),
                PackageMaxMembers = 10,
                PackageMaxAssistantCoaches = 1,
                PackageFeatures = new List<string>
                {
                    "Basic workout builder functionality",
                    "Access to nutrition services",
                    "Basic synced services"
                },
                IsPublic = true
            },
            new AllTeamPackages()
            {
                PackageID = 1,
                PackageName = "Pro Package",
                PackageDescription = "Our most premium package with all the benefits. Great for full time coaches and organizations",
                PackageSubtext = "For the Extreme",
                PackagePriceMonthly = new decimal (30.00),
                PackagePriceYearly = new decimal (300),
                PackageMaxMembers = 100,
                PackageMaxAssistantCoaches = 3,
                PackageFeatures = new List<string>
                {
                    "For large scale clubs and teams",
                    "Full workout builder access",
                    "Injury reporting features",
                    "Access to photo and video storage",
                    "Custom automated reports"
                },
                IsPublic = true
            },
        };
    }
}