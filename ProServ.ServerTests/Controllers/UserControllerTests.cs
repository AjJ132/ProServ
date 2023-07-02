using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProServ.Server.Controllers;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using ProServ.Server.Contexts;
using ProServ.Shared.Models.UserInfo;
using Microsoft.Extensions.Options;

namespace ProServ.Server.Controllers.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private IDbContextFactory<ProServDbContext> _contextFactory;
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Create a mock user store - needed to construct a UserManager
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();

            // Create a mock UserManager - we can use this to control the behavior of the UserManager in tests
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Create an instance of the controller, passing in the mock UserManager
            _controller = new UserController(null, _userManagerMock.Object);
        }

        [TestMethod]
        public async Task GetUserRoleTest()
        {
            // Arrange
            var testUserId = "TestUser";
            var testRole = "TestRole";

            var user = new IdentityUser { Id = testUserId, UserName = "TestUserName" };

            // Create a ClaimsPrincipal (User) to simulate the authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUserId),
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new[] { testRole });

            // Act
            var result = await _controller.GetUserRole();

            // Assert
            var returnedRole = result.Value;
            Assert.IsNotNull(returnedRole);
            Assert.AreEqual(testRole, returnedRole);
        }

        [TestMethod]
        public async Task CheckIfProfileExistsTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProServDbContext>()
                .UseInMemoryDatabase(databaseName: "CheckIfProfileExistsTest") // use a unique name for each test
                .Options;

            var testUserId = "TestUser";
            var user = new IdentityUser { Id = testUserId, UserName = "TestUserName" };
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, testUserId),
            }));

            // Create a new user profile and save it in the in-memory database
            using (var context = new ProServDbContext(options))
            {
                context.UserProfile.Add(new UserProfile { UserId = testUserId });
                await context.SaveChangesAsync();
            }

            // We create a new instance of the controller with the DbContext that uses the in-memory database
            var userManager = new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            var controller = new UserController(_contextFactory, userManager.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await controller.CheckIfProfileExists();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var exists = (bool)okResult.Value;
            Assert.IsTrue(exists);
        }



    }
}
