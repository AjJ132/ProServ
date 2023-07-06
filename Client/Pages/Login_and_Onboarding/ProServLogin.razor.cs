
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using ProServ.Shared.Models.NET_CORE_USER;
using System.Net.Http.Json;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using System.Text.Json;

namespace ProServ.Client.Pages.Login_and_Onboarding.Login
{
    public partial class ProServLogin
    {
        [Inject]
        HttpClient Http { get; set; }

        [Inject]
        Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }


        private LoginUser _loginUser = new LoginUser();
        private string _email = "";
        private string _password = "";
        private bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var token = await localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"LoginPage() Retrieved token: {token}");
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                //TODO Validate that user is still registered in the system 
                if (authState.User.Identity.IsAuthenticated)
                {
                    // User is authenticated
                    //Check users roles
                    var roleResponse = await Http.GetAsync("api/Auth/user-role");
                    if (roleResponse.IsSuccessStatusCode)
                    {
                        var role = await roleResponse.Content.ReadAsStringAsync();
                        //navigate based off role
                        if (role.Equals("Coach"))
                        {
                            NavigationManager.NavigateTo("/Coaches-Dashboard");
                        }
                        else if (role.Equals("Member"))
                        {
                            NavigationManager.NavigateTo("/Dashboard");
                        }
                        else if (role.Equals("Sudo"))
                        {
                            NavigationManager.NavigateTo("/Dashboard");
                        }
                        else
                        {
                            Debug.WriteLine("Error finding users role");
                            Console.WriteLine("Error finding users role");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Error Getting users role");
                        Console.WriteLine("Error Getting users role");
                    }
                }
                else
                {
                    // User is not authenticated
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                this._loading = false;
                await base.OnInitializedAsync();
            }
        }

        private void FillNLogin()
        {
            _email = "aj132@icloud.com";
            _password = "Johnson132";
            HandleValidSubmit();
        }

        private void CoachLogin()
        {
            _email = "aj132@icloud.com";
            _password = "Johnson132";
            HandleValidSubmit();
        }

        private async Task HandleValidSubmit()
        {
            this._loginUser.Email = _email;
            this._loginUser.Password = _password;
            var response = await Http.PostAsJsonAsync("api/Auth/Login", _loginUser);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenObj = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                await localStorage.SetItemAsync("token", tokenObj["token"]);
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                if (authState.User.Identity.IsAuthenticated)
                {
                    //TODO Error handling for finding userid
                    //get user id from custom AuthenticationStateProvider
                    
                    //check for users role
                    var userRolesResponse = await Http.GetAsync("api/Auth/user-role");
                            
                    if(userRolesResponse.IsSuccessStatusCode)
                    {
                        var userRole = await userRolesResponse.Content.ReadAsStringAsync();
                        if (userRole == "Coach")
                        {
                            // Navigate to home page if they are returning user
                            NavigateToCoachesDashboard();
                        }
                        else if (userRole == "Member")
                        {
                            // Navigate to onboarding process if they are new user or previously exited process
                            NavigateToHome();
                        }
                        else if (userRole == "Sudo")
                        {
                            // Navigate to onboarding process if they are new user or previously exited process
                            NavigateToHome();
                        }
                        else
                        {
                            Console.WriteLine("Error finding user role");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error 1005: ProServ Login");
                }
            }
            else
            {
                Console.WriteLine("Error logging in");
            }
        }

        private void NavigateToHome()
        {
            NavigationManager.NavigateTo("/Dashboard");
        }

        private void NavigateToCoachesDashboard()
        {
            NavigationManager.NavigateTo("/Coaches-Dashboard");
        }

        private void NavigateToOnboarding()
        {
            NavigationManager.NavigateTo("/Onboarding");
        }

        private void NavigateToRegister()
        {
            NavigationManager.NavigateTo("/Register");
        }
    }
}