
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Radzen.Blazor;
using System.Text.RegularExpressions;
using ProServ.Shared.Models.NET_CORE_USER;
using ProServ.Shared.Models.UserInfo;

namespace ProServ.Client.Pages.Login_and_Onboarding
{
    public partial class Register
    {
        //register selection
        private int _registerStep = 1;
        private bool _coachOrAthlete = false;


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
        private RegisterUser _registerUser = new RegisterUser();


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

        private void NavigateToOnboarding()
        {
            NavigationManager.NavigateTo("/Onboarding");
        }

        private async void HandleValidSubmit()
        {
            //Check to make sure email and password are valid
            string _email = email;
            string _password = password;
            string _confirmPassword = confirmPassword;
            if (_email == null || _password == null || _confirmPassword == null)
            {
                return;
            }

            if (!password.Equals(confirmPassword))
            {
                this._passwordMatch = false;
                return;
            }
            else
            {
                //Double check password strength
                if (!ValidatePassword(password))
                {
                    return;
                }
            }

            _registerUser.Email = _email;
            _registerUser.Password = _password;
            //register user and then get there id back
            var response = await Http.PostAsJsonAsync("api/Auth/Register", _registerUser);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var token = content["token"];
                var userId = content["userId"];
                //Get JWT and Save to sessionn storage
                Console.WriteLine($"Received JWT token: {token}"); // Add this line
                await localStorage.SetItemAsync("token", token);
                await AuthProvider.GetAuthenticationStateAsync();
                //Create Onboarding variable
                ProfileOnboarding onBoarding = new ProfileOnboarding
                {
                    UserId = userId,
                    Completed = false
                };
                var postOnBoard = await Http.PostAsJsonAsync("api/User/Onboarding", onBoarding);
                if (!postOnBoard.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error in marking onboarding status");
                }

                NavigateToOnboarding();
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
    
        private void RegisterSwitchToggle()
        {
            StateHasChanged();
        }
    
    }
}