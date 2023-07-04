
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Radzen.Blazor;
using System.Text.RegularExpressions;
using ProServ.Shared.Models.NET_CORE_USER;
using ProServ.Shared.Models.UserInfo;
using Radzen;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace ProServ.Client.Pages.Login_and_Onboarding
{
    public partial class Register
    {
        //register selection
        private int _registerStep = 1;
        private bool _isCoach = false; 
        private RegisterUser registerUser = new RegisterUser();


        private string _firstName;
        private string _lastName;
        private string email;
        private string password;
        private string confirmPassword;
        private Regex _emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        private bool _enterEmail = false;
        private bool _enterPassword = false;
        private bool _passwordMatch = false;
        private Regex _numberRegex = new Regex(@"[0-9]+");
        private bool hasNumber = false;
        private Regex _upperCharRegex = new Regex(@"[A-Z]+");
        private bool _hasUpperChar = false;
        private Regex _lowerCharRegex = new Regex(@"[a-z]+");
        private bool _hasLowerChar = false;
        private Regex _minCharRegex = new Regex(@".{8,}");
        private bool _hasMinChars = false;
        private RadzenText _lowercaseCheck;
        private RadzenText _numberCheck;
        private RadzenText _minCharCheck;
        private RadzenText _uppercaseCheck;

        UserInformation _userInformation = new UserInformation();
        UserProfile _userProfile = new UserProfile();
        string _phoneNumber = "";
        int _heightFeet = 0;
        int _heightInches = 0;
        List<string> _genders = new List<string>
        {
            "Male",
            "Female",
            "Perfer not to say"
        };
        bool _savingUserInformation = false;
        bool _savingUserProfile = false;
        bool _completedUserInformation = true;
        bool _completedUserProfile = false;
        private int currentStep = 1;


        protected override void OnInitialized()
        {
            _lowercaseCheck = new RadzenText();
            _numberCheck = new RadzenText();
            _minCharCheck = new RadzenText();
            _uppercaseCheck = new RadzenText();
            _lowercaseCheck.Style = "color: Red;";
            _numberCheck.Style = "color: Red;";
            _minCharCheck.Style = "color: Red;";
            _uppercaseCheck.Style = "color: Red;";
            base.OnInitialized();
        }

        //validate password strength
        //validate password strength
        private void ValidatePassword()
        {
            //Check password to see if it meets requirements
            //Requirements: 8 characters, 1 uppercase, 1 lowercase, 1 number,
            if (string.IsNullOrEmpty(password))
            {
                //Set all validation flags to false
                hasNumber = false;
                _hasUpperChar = false;
                _hasLowerChar = false;
                _hasMinChars = false;
                return;
            }

            //Check for number
            if (_numberRegex.IsMatch(password))
            {
                hasNumber = true;
                this._numberCheck.Style = "color: Green;";
            }
            else
            {
                hasNumber = false;
                this._numberCheck.Style = "color: Red;";
            }

            //check for uppercase
            if (_upperCharRegex.IsMatch(password))
            {
                _hasUpperChar = true;
                this._uppercaseCheck.Style = "color: Green;";
            }
            else
            {
                _hasUpperChar = false;
                this._uppercaseCheck.Style = "color: Red;";
            }

            //check for lowercase
            if (_lowerCharRegex.IsMatch(password))
            {
                _hasLowerChar = true;
                this._lowercaseCheck.Style = "color: Green;";
            }
            else
            {
                _hasLowerChar = false;
                this._lowercaseCheck.Style = "color: Red;";
            }

            //check for min chars
            if (_minCharRegex.IsMatch(password))
            {
                _hasMinChars = true;
                this._minCharCheck.Style = "color: Green;";
            }
            else
            {
                _hasMinChars = false;
                this._minCharCheck.Style = "color: Red;";
            }
        }

        private bool ValidatePassword(string ps)
        {
            //Check password to see if it meets requirements
            //Requirements: 8 characters, 1 uppercase, 1 lowercase, 1 number,
            if (string.IsNullOrEmpty(ps))
            {
                //Set all validation flags to false
                hasNumber = false;
                _hasUpperChar = false;
                _hasLowerChar = false;
                _hasMinChars = false;
                return false;
            }

            //Check for number
            if (_numberRegex.IsMatch(ps))
            {
                hasNumber = true;
                this._numberCheck.Style = "color: Green;";
            }
            else
            {
                hasNumber = false;
                this._numberCheck.Style = "color: Red;";
                return false;
            }

            //check for uppercase
            if (_upperCharRegex.IsMatch(ps))
            {
                _hasUpperChar = true;
                this._uppercaseCheck.Style = "color: Green;";
            }
            else
            {
                _hasUpperChar = false;
                this._uppercaseCheck.Style = "color: Red;";
                return false;
            }

            //check for lowercase
            if (_lowerCharRegex.IsMatch(ps))
            {
                _hasLowerChar = true;
                this._lowercaseCheck.Style = "color: Green;";
            }
            else
            {
                _hasLowerChar = false;
                this._lowercaseCheck.Style = "color: Red;";
                return false;
            }

            //check for min chars
            if (_minCharRegex.IsMatch(ps))
            {
                _hasMinChars = true;
                this._minCharCheck.Style = "color: Green;";
            }
            else
            {
                _hasMinChars = false;
                this._minCharCheck.Style = "color: Red;";
                return false;
            }

            return true;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrEmpty(email))
            {
                _enterEmail = false;
                return;
            }

            if (_emailRegex.IsMatch(email))
            {
                _enterEmail = false;
            }
            else
            {
                _enterEmail = true;
            }
        }

        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("/");
        }

        private async void HandleValidSubmit()
        {
            
            //TODO validate that email isnt taken

            //check if is athlete or coach
            if(this._isCoach)
            {
                registerUser.IsCoach = true;
            }
            else
            {
                registerUser.IsCoach = false;
            }

            try
            {
                //register user and then get there id back
                var response = await Http.PostAsJsonAsync("api/Auth/Register", registerUser);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    var token = content["token"];
                    var userId = content["userId"];
                    //Get JWT and Save to sessionn storage
                    Console.WriteLine($"Received JWT token: {token}"); // Add this line
                    await localStorage.SetItemAsync("token", token);
                    await AuthProvider.GetAuthenticationStateAsync();
                    
                    var userInfo = new UserInformation { FirstName = registerUser.FirstName, LastName = registerUser.LastName, UserId = userId, 
                                                        ActiveUser = true, DateCreated = DateTime.Now, LastAccessed = DateTime.Now };
                    if(registerUser.IsCoach)
                    {
                        userInfo.UserType = "Coach";
                    }
                    else
                    {
                        userInfo.UserType = "Athlete";
                    }

                    //Save first and last name to User Information Table
                    //TODO Fix user information not being inserted
                    var userInformationResponse = await Http.PostAsJsonAsync("api/User/UserInformation", userInfo);
                    if(!userInformationResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("There was an error saving the user information");
                        Debug.WriteLine(userInformationResponse.Content.ReadAsStringAsync());
                    }

                    if(registerUser.IsCoach)
                    {
                        //Navigation to coaches dashboard
                        NavigationManager.NavigateTo("/Coaches-Dashboard");
                    }
                    else
                    {
                        NavigationManager.NavigateTo("/Dashboard");
                    }
                }
                else
                {
                    //TODO: Add error handling
                    Console.WriteLine("There was an error registering the user");
                    //Display error message
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    

       
    }
}