using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProServ.Server.Contexts;
using ProServ.Server.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using ProServ.Shared.Models.Coaches;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ProServ.Shared.Models.UserInfo;
using Microsoft.Extensions.Options;

namespace ProServ.Tests
{
    public class UserControllerTest
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IDbContextFactory<ProServDbContext>> _contextFactoryMock;
        private readonly UserController _controller;

        //Variables to be reused
        private Team _testTeam;

        //Setup
        public UserControllerTest()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _contextFactoryMock = new Mock<IDbContextFactory<ProServDbContext>>();

            _controller = new UserController(_contextFactoryMock.Object, _userManagerMock.Object);

            SetupUserForController();
            CreateReusedVariables();


        }

        public void SetupUserForController()
        {
            // Identity User
            var identityUser = new IdentityUser
            {
                Id = "1",
                UserName = "Test User",
                Email = "testing@gmail.com",
            };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(identityUser);

            // User Claims
            var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        // Add any other claims as needed.
    };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Mock HttpContext
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.User).Returns(principal);

            // Mock ControllerContext
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Object.HttpContext = httpContextMock.Object;

            _controller.ControllerContext = controllerContextMock.Object;
        }

        public void RemoveUserForController()
        {
            // Null user
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);
        }

        private void CreateReusedVariables()
        {
            _testTeam = new Team()
            {
                TeamID = 1,
                TeamName = "Test Team",
                Location = "Test Location",
                CoachesCode = "Test Code",
                UsersCode = "Test Code",
                Terminated = false,
                OwnerID = "1",
                TeamInfo = new TeamInfo()
                {
                    TeamID = 1,
                    DateCreated = DateTime.Now,
                    OwnerID = "1",
                    IsSchoolOrganization = false,
                    TeamPackageID = 4,
                    TimeChanged = 0,
                    TeamSport = "Track and Field"
                },
                TeamPackage = new TeamPackage()
                {
                    TeamID = 1,
                    PackageID = 4,
                    PackageStart = DateTime.Now,
                    PackageEnd = DateTime.Now.AddDays(30),
                }

            };
        }

        private void ResetUserForController()
        {
            // Mock HttpContext
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.User).Returns<ClaimsPrincipal>(null);

            // Mock ControllerContext
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Object.HttpContext = httpContextMock.Object;

            _controller.ControllerContext = controllerContextMock.Object;
        }


        //Tests
        [Fact]
        public async Task TestIfUserProfileExists()
        {
            var options = new DbContextOptionsBuilder<ProServDbContext>()
             .EnableSensitiveDataLogging()
             .UseInMemoryDatabase(databaseName: "CheckIfUserPofileExistsDatabase").Options;
            var context = new ProServDbContext(options);

            var user1 = new UserProfile()
            {
                UserId = "1",
            };
            var user2 = new UserProfile()
            {
                UserId = "2",
            };

            context.UserProfile.Add(user1);
            context.UserProfile.Add(user2);

            await context.SaveChangesAsync();

            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));


            var passResult = await _controller.CheckIfProfileExists();

            //First remove user from user manager
            RemoveUserForController();
            var failResult = await _controller.CheckIfProfileExists();

            //Assert
            var actionResultPass = Assert.IsType<ActionResult<bool>>(passResult);
            var actionResultFail = Assert.IsType<ActionResult<bool>>(failResult);

            var resultPass = Assert.IsType<OkObjectResult>(actionResultPass.Result);
            var resultFail = Assert.IsType<NotFoundObjectResult>(actionResultFail.Result);
            Assert.Equal(404, resultFail.StatusCode);
        }
    }
}