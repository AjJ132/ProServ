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
using Microsoft.JSInterop;
using ProServ.Client;
using ProServ.Client.Shared;
using Radzen;
using Radzen.Blazor;
using ProServ.Client.Data;
using ProServ.Shared.Models.Coaches;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ProServ.Client.Controllers;
using ProServ.Shared.Models.UserInfo;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace ProServ.Client.Pages
{
    public partial class CoachesDashboard
    {
        
        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
        }

       
    }
}