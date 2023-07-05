
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

        UserInformation _userInformation = new UserInformation();
        UserProfile _userProfile = new UserProfile();
       
        bool _savingUserInformation = false;
        bool _savingUserProfile = false;
        bool _completedUserInformation = true;
        bool _completedUserProfile = false;
        private int currentStep = 1;


        protected override void OnInitialized()
        {
            base.OnInitialized();
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
                    
                    //this is temporary. If its null then it defaults to lowest min year which is not supported in DB
                    //TODO: Fix this
                    userInfo.Birthday = DateTime.Now.AddYears(-100);

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