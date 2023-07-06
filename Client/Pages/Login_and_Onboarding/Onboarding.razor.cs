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
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using ProServ.Client;
using ProServ.Client.Shared;
using Radzen.Blazor;
using ProServ.Client.Data;
using ProServ.Shared.Models.Coaches;
using ProServ.Client.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Radzen;
using ProServ.Shared.Models.UserInfo;
using System.Text.Json;
using System.Text;

namespace ProServ.Client.Pages.Login_and_Onboarding
{
    public partial class Onboarding
    {
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
        protected override async Task OnInitializedAsync()
        {
            //Check to see if they have already been onboarded and navigate them to dashboard if they are
            //First get userId
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            string id = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //if no ID then throw error
            // TODO: implement error handling
            //check to see if ID has already completed onboarding status
            var onboardingStatus = await Http.GetAsync($"api/User/Onboarding/Completed");
            if (onboardingStatus.IsSuccessStatusCode)
            {
                var didComplete = await onboardingStatus.Content.ReadFromJsonAsync<bool>();
                if (didComplete)
                {
                    Console.WriteLine("User already completed Onboarding proccess");
                    NavigationManager.NavigateTo("/Dashboard");
                }
                else
                {
                    //check to see if they completed first step of proccess by checking for user information table TODO
                    var userInfoExists = await Http.GetAsync($"api/User/user-information/exists");
                    if (userInfoExists.IsSuccessStatusCode)
                    {
                        var hasUserInfo = await userInfoExists.Content.ReadFromJsonAsync<bool>();
                        if (hasUserInfo)
                        {
                            //its backwards
                            this._completedUserInformation = false;
                            this._completedUserProfile = true;
                            currentStep++;
                        }
                    }

                    Console.WriteLine("Starting Onboarding proccess");
                }
            }
            else
            {
                Console.WriteLine("Error finding user in database");
                NavigationManager.NavigateTo("");
            }
        }

        private async void SubmitUserInfo(UserInformation userInformation)
        {
            //get user id
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            string userID = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            userInformation.UserId = userID;
            //concatenate height
            userInformation.Height = _heightFeet.ToString() + "'" + _heightInches.ToString() + "\"";
            //Set ReportsTo to Default user (Sarah) might need to change in the future
            userInformation.ReportsTo = "";
            userInformation.UserType = "Member";
            userInformation.DateCreated = DateTime.Now;
            userInformation.LastAccessed = DateTime.Now;
            userInformation.ActiveUser = true;
            userInformation.TeamID = 0;
            //First put phone number then post user information
            var phoneNumberContent = new StringContent(JsonSerializer.Serialize(_phoneNumber), Encoding.UTF8, "application/json");
            Console.WriteLine(phoneNumberContent.ToString());
            var putPhoneNumber = await Http.PutAsJsonAsync($"api/auth/PhoneNumber", _phoneNumber);
            //Check if phone number was successfully updated
            if (!putPhoneNumber.IsSuccessStatusCode)
            {
                Console.WriteLine("Error updating phone number");
            }
            else
            {
                var postUserInfo = await Http.PostAsJsonAsync("api/User/UserInformation", userInformation);
                //Check if user information was successfully posted
                if (!postUserInfo.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error posting user information");
                }
                else
                {
                    //its backwards
                    this._completedUserInformation = false;
                    this._completedUserProfile = true;
                    currentStep++;
                    StateHasChanged();
                }
            }
        }

        private async void SubmitUserProfile(UserProfile userProfile)
        {
            if (_completedUserProfile)
            {
                //TODO error handling for userid
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                string userID = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                userProfile.UserId = userID;
                var response = await Http.PostAsJsonAsync("api/User/Profile", userProfile);
                //Check if user profile was successfully posted
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error posting user profile");
                }
                else
                {
                    //if completed move to home page
                    //its backwards
                    if (!_completedUserInformation && _completedUserProfile)
                    {
                        //if True update UserOnboarding page then navigate to HomePage
                        bool status = true;
                        var updateOnboarding = await Http.PutAsJsonAsync($"api/User/Onboarding/Complete", status);
                        UserTrackRecords newRecords = new UserTrackRecords
                        {
                            UserId = userID
                        };
                        var insertUserRecordsRequest = await Http.PostAsJsonAsync("api/User/track-records", newRecords);
                        if (!updateOnboarding.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Error updating UserOnboarding");
                        }
                        else
                        {
                            Console.WriteLine("UserOnboarding updated");
                            NavigationManager.NavigateTo("/Dashboard");
                        }
                    }
                }
            }
        }

        private void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            Console.WriteLine("Invalid Submit");
        }

        private void AutoFill()
        {
            this._userInformation.FirstName = "Sarah";
            this._userInformation.LastName = "Hendrick";
            this._userInformation.Birthday = new DateTime(2000, 07, 04);
            this._phoneNumber = "470-281-8766";
            this._userInformation.City = "Kennesaw";
            this._userInformation.State = "GA";
            this._userInformation.Height = "5'7\"";
            this._userInformation.Weight = 135;
            this._userInformation.Gender = "Female";
            this._userInformation.IsInHighschool = false;
            this._userInformation.School = "";
        }
    }
}