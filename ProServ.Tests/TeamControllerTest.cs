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

namespace ProServ.Tests
{
    public class TeamControllerTest
    {
        private readonly UserManager<IdentityUser> _userManagerMock;
        private readonly Mock<IDbContextFactory<ProServDbContext>> _contextFactoryMock;

        public TeamControllerTest()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new UserManager<IdentityUser>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _contextFactoryMock = new Mock<IDbContextFactory<ProServDbContext>>();
        }

        [Fact]
        public async Task CanGetAllTeamPackagesAsync()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProServDbContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "GetPackagesDatabase").Options;

            var package = new AllTeamPackages {
                PackageID = 1,
                IsPublic = true, 
                PackageName ="test package",
                PackageDescription = "test description",
                PackageSubtext = "test subtext",
                PackagePriceMonthly = 10.00M,
                PackagePriceYearly = 100.00M,
                PackageMaxMembers = 10,
                PackageMaxAssistantCoaches = 2
                
            };

            // Insert seed data into the database using one instance of the context
            using (var context = new ProServDbContext(options))
            {
                context.AllTeamPackages.Add(package);
                context.SaveChanges();
            }

            // Setup your context factory mock to return your context with seeded data
            _contextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(new ProServDbContext(options));

            var controller = new TeamController(_contextFactoryMock.Object, _userManagerMock);

            // Act
            var result = await controller.GetAllTeamPackagesAsync();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<AllTeamPackages>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsAssignableFrom<List<AllTeamPackages>>(okResult.Value);

            Assert.Single(model);
            Assert.Equal(package, model.First());
        }

    }
}
