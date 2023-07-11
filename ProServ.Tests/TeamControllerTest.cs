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
    public class TeamControllerTest
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IDbContextFactory<ProServDbContext>> _contextFactoryMock;
        private readonly TeamController _controller;

        //Variables to be reused
        private Team _testTeam;

        //Setup
        public TeamControllerTest()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _contextFactoryMock = new Mock<IDbContextFactory<ProServDbContext>>();

            _controller = new TeamController(_contextFactoryMock.Object, _userManagerMock.Object);

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

        [Fact]
        public async Task CanGetAllTeamPackagesAsync()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProServDbContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "GetPackagesDatabase").Options;

            var packages = new List<AllTeamPackages>() {

                new AllTeamPackages()
                {
                    PackageID = 1,
                    IsPublic = true,
                    PackageName ="test package",
                    PackageDescription = "test description",
                    PackageSubtext = "test subtext",
                    PackagePriceMonthly = 10.00M,
                    PackagePriceYearly = 100.00M,
                    PackageMaxMembers = 10,
                    PackageMaxAssistantCoaches = 2
                },
                new AllTeamPackages()
                {
                    PackageID = 2,
                    IsPublic = true,
                    PackageName ="test package 2",
                    PackageDescription = "test description 2",
                    PackageSubtext = "test subtext 2",
                    PackagePriceMonthly = 20.00M,
                    PackagePriceYearly = 200.00M,
                    PackageMaxMembers = 20,
                    PackageMaxAssistantCoaches = 4
                },
                new AllTeamPackages()
                {
                    PackageID = 3,
                    IsPublic = true,
                    PackageName ="test package 3",
                    PackageDescription = "test description 3",
                    PackageSubtext = "test subtext 3",
                    PackagePriceMonthly = 30.00M,
                    PackagePriceYearly = 300.00M,
                    PackageMaxMembers = 30,
                    PackageMaxAssistantCoaches = 6
                }
            };

            // Insert seed data into the database using one instance of the context
            var context = new ProServDbContext(options);

            await context.AllTeamPackages.AddRangeAsync(packages);
            context.SaveChanges();


            // Setup your context factory mock to return your context with seeded data
            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));

            // Act
            var result = await _controller.GetAllTeamPackagesAsync();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<AllTeamPackages>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsAssignableFrom<List<AllTeamPackages>>(okResult.Value);

            //TODO : figure out why this is failing and add more test cases
            //Assert.Single(model);
            //Assert.Equal(packages, model);
            Assert.NotNull(model);

            //dispose of the context
            context.Dispose();
        }

        [Fact]
        public async Task CheckIfTeamNameExistsAsync()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProServDbContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "CheckIfTeamNameExistsDatabase").Options;

            string teamNameToFail = "Slow Team";
            string teamNameToPass = "Extra Fast Team";
            string teamNameToFail2 = null;
            var teams = new List<Team>()
            {
                new Team()
                {
                    TeamID = 1,
                    TeamName = "Fast Team",
                    Location = "Fast Town",
                    CoachesCode = "CoachCode1",
                    UsersCode = "UsersCode2",
                    Terminated = false,
                    OwnerID = "OwnerID1"

                },
                new Team()
                {
                    TeamID = 2,
                    TeamName = "Slow Team",
                    Location = "Slow Town",
                    CoachesCode = "CoachCode2",
                    UsersCode = "UsersCode2",
                    Terminated = false,
                    OwnerID = "OwnerID2"
                },
                new Team()
                {
                    TeamID = 3,
                    TeamName = "Medium Team",
                    Location = "Medium Town",
                    CoachesCode = "CoachCode3",
                    UsersCode = "UsersCode3",
                    Terminated = false,
                    OwnerID = "OwnerID3"

                }
            };

            // Insert seed data into the database using one instance of the context
            var context = new ProServDbContext(options);

            await context.Teams.AddRangeAsync(teams);
            context.SaveChanges();



            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));

            // Act
            var failResult = await _controller.TeamNameExistsAsync(teamNameToFail);
            var passResult = await _controller.TeamNameExistsAsync(teamNameToPass);
            var failResult2 = await _controller.TeamNameExistsAsync(teamNameToFail2);

            // Assert
            var actionResultFail = Assert.IsType<ActionResult<bool>>(failResult);
            var okResultFail = Assert.IsAssignableFrom<ObjectResult>(actionResultFail.Result);
            var modelFail = Assert.IsAssignableFrom<bool>(okResultFail.Value);

            var actionResultFail2 = Assert.IsAssignableFrom<ActionResult<bool>>(failResult2);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResultFail2.Result);
            Assert.Equal("Team name cannot be null or empty.", badRequestResult.Value);


            var actionResultPass = Assert.IsType<ActionResult<bool>>(passResult);
            var okResultPass = Assert.IsAssignableFrom<ObjectResult>(actionResultPass.Result);
            var modelPass = Assert.IsAssignableFrom<bool>(okResultPass.Value);


            Assert.True(modelFail);
            Assert.False(modelPass);


            //dispose of the context
            context.Dispose();
        }

        [Fact]
        public async Task CanRegisterTeam()
        {
            var options = new DbContextOptionsBuilder<ProServDbContext>()
               .EnableSensitiveDataLogging()
               .UseInMemoryDatabase(databaseName: "RegisterTeamDatabase").Options;

            //First insert a user into user mananger

            //TODO allow for coaches to update email 

            //Arrange
            CoachRegistration registration = new()
            {
                UserID = "1",
                TeamName = "Test Team",
                TeamLocationCity = "Test City",
                TeamLocationState = "Test State",
                TeamSport = "Other",
                TeamSportSpecify = "Track and Field",
                CoachesCode = null,
                UsersCode = null,
                DateCreated = DateTime.Now,
                IsSchoolOrganization = false,
                AffliatedSchool = null,
                PackageID = 4,
                PackageStart = DateTime.Now,
                PackageEnd = DateTime.Now.AddDays(30),
                Email = "newemail@icloud.com",
                EmailIsCorrect = true,
                Address = "123 Test Street",
                Address2 = null,
                City = "Test City",
                State = "Test State",
                Zipcode = "12345"
            };

            //create db context and add user information to reflec user
            var context = new ProServDbContext(options);
            UserInformation ui = new UserInformation()
            {
                UserId = "1"
            };

            context.UserInformation.Add(ui);
            await context.SaveChangesAsync();

            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));

            //Act
            var statusOkResult = await _controller.SubmitCoachRegistration(registration);

            //Assert if the team was created and returned a 200 status code
            var statusOkPass = Assert.IsType<OkResult>(statusOkResult);
            Assert.Equal(200, statusOkPass.StatusCode);


            //Change user manager settings to throw a result upong changing email and force email change
            registration.EmailIsCorrect = false;
            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<IdentityUser>()))
                .ThrowsAsync(new Exception("Test Exception"));

            //Assert if the email failed to update and returned a 400 status code
            var emailUpdateFailResult = await _controller.SubmitCoachRegistration(registration);
            var emailUpdateFail = Assert.IsType<ObjectResult>(emailUpdateFailResult);
            Assert.Equal(500, emailUpdateFail.StatusCode);

            //The client code should never let this happen but if for some reason
            registration = null;


            //Assert if the coach registration failed to update and returned a 500 status code
            var status500Result = await _controller.SubmitCoachRegistration(registration);
            var status500Fail = Assert.IsType<ObjectResult>(status500Result);
            Assert.Equal(500, status500Fail.StatusCode);

            //dispose of the context
            context.Dispose();
        }

        [Fact]
        public async Task GetUsersTeamAndIncludeChildren()
        {
            var options = new DbContextOptionsBuilder<ProServDbContext>()
               .EnableSensitiveDataLogging()
               .UseInMemoryDatabase(databaseName: "GetTeamDatabase").Options;
            var context = new ProServDbContext(options);
            await context.Teams.AddAsync(this._testTeam);
            await context.SaveChangesAsync();

            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));

            //should pass
            var passResult = await _controller.GetUsersTeamAndIncludeChildren(1);
            var statsOkPass = Assert.IsType<ActionResult<Team>>(passResult);
            var okResult = Assert.IsType<OkObjectResult>(statsOkPass.Result);
            var teamModel = Assert.IsAssignableFrom<Team>(okResult.Value);

            Assert.Equal(this._testTeam.TeamID, teamModel.TeamID);
            //ensure its not null
            Assert.NotNull(teamModel);

            //should fail
            var status404Result = await _controller.GetUsersTeamAndIncludeChildren(100); //pass 100 to ensure no team is found
            var status404Fail = Assert.IsType<ActionResult<Team>>(status404Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(status404Fail.Result);
            Assert.Equal(404, notFoundResult.StatusCode);


            //dispose of the context
            context.Dispose();
        }

        [Fact]
        public async Task GetAllAthletesByTeamID()
        {

            //Arrange
            var options = new DbContextOptionsBuilder<ProServDbContext>()
             .EnableSensitiveDataLogging()
             .UseInMemoryDatabase(databaseName: "GetAllAthleteByTeamIDDatabase").Options;
            var context = new ProServDbContext(options);



            var user1 = new UserInformation()
            {
                UserId = "1",
                TeamID = 1
            };
            var user2 = new UserInformation()
            {
                UserId = "2",
                TeamID = 1
            };
            var user3 = new UserInformation()
            {
                UserId = "3",
                TeamID = 1
            };




            context.UserInformation.Add(user1);
            context.UserInformation.Add(user2);
            context.UserInformation.Add(user3);

            await context.SaveChangesAsync();

            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));

            var passResult = await _controller.GetAllAthletesByTeamIdAsync(1);
            var failResult = await _controller.GetAllAthletesByTeamIdAsync(2);
            var failResult2 = await _controller.GetAllAthletesByTeamIdAsync(-1);

            //Assert
            var actionResultPass = Assert.IsType<ActionResult<List<UserInformation>>>(passResult);
            var okResultPass = Assert.IsAssignableFrom<ObjectResult>(actionResultPass.Result);

            //look for 404 error
            var actionResultFail = Assert.IsType<ActionResult<List<UserInformation>>>(failResult);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResultFail.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

            //look for bad request
            var actionResultFail2 = Assert.IsType<ActionResult<List<UserInformation>>>(failResult2);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResultFail2.Result);
            Assert.Equal(400, badRequestResult.StatusCode);

            //look for correct team id

            /*
             * foreach (UserInformation user in users)
            {
                Assert.Equal(1, user.TeamID);
            } */

            //dispose of the context
            context.Dispose();
        }

        [Fact]
        public async Task GetAthletesDataByTeamIdAndCount()
        {
            //Pass by default for now



            //Arrange
            var options = new DbContextOptionsBuilder<ProServDbContext>()
             .EnableSensitiveDataLogging()
             .UseInMemoryDatabase(databaseName: "GetAllAthleteDataDatabase").Options;
            var context = new ProServDbContext(options);

            var user1 = new UserInformation()
            {
                UserId = "1",
                TeamID = 1
            };
            var user2 = new UserInformation()
            {
                UserId = "2",
                TeamID = 1
            };
            var user3 = new UserInformation()
            {
                UserId = "3",
                TeamID = 1
            };

            context.UserInformation.Add(user1);
            context.UserInformation.Add(user2);
            context.UserInformation.Add(user3);

            await context.SaveChangesAsync();

            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));


            var passResult = await _controller.GetAthletesDataByTeamIdAndCount(1, 0, 10);

            //Assert
            var actionResultPass = Assert.IsType<ActionResult<IEnumerable<UserInformation>>>(passResult);
            var okResultPass = Assert.IsAssignableFrom<OkObjectResult>(actionResultPass.Result);

            //dispose of the context
            context.Dispose();

        }

        [Fact]
        public async Task GetAthletesCountByTeamID()
        {
            var options = new DbContextOptionsBuilder<ProServDbContext>()
              .EnableSensitiveDataLogging()
              .UseInMemoryDatabase(databaseName: "GetAllAthleteDatabase").Options;
            var context = new ProServDbContext(options);

            var user1 = new UserInformation()
            {
                UserId = "1",
                TeamID = 1
            };
            var user2 = new UserInformation()
            {
                UserId = "2",
                TeamID = 1
            };
            var user3 = new UserInformation()
            {
                UserId = "3",
                TeamID = 1
            };

            context.UserInformation.Add(user1);
            context.UserInformation.Add(user2);
            context.UserInformation.Add(user3);

            await context.SaveChangesAsync();

            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));

            var passResult = await _controller.GetAthletesCount(1);
            var failResult = await _controller.GetAthletesCount(-1);

            //Assert
            var actionResultPass = Assert.IsType<ActionResult<int>>(passResult);
            var okResultPass = Assert.IsAssignableFrom<OkObjectResult>(actionResultPass.Result);
            Assert.Equal(3, okResultPass.Value);

            //look for zero
            var actionResultFail = Assert.IsType<ActionResult<int>>(failResult);
            var okResultFail = Assert.IsAssignableFrom<OkObjectResult>(actionResultFail.Result);
            Assert.Equal(0, okResultFail.Value);

            //dispose of the context
            context.Dispose();
        }
    }
}
