using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProServ.Server.Contexts;
using ProServ.Server.Controllers;
using ProServ.ServerTests.Contexts;
using ProServ.Shared.Models.UserInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProServ.Server.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        [TestMethod()]
        public void UserControllerTest()
        {
            
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserRoleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckIfProfileExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public async Task GetUserProfileTest()
        {
            // Arrange


            // Define the user id
            var testId = "testId";

            // mocking User
            var mockPrincipal = new Mock<ClaimsPrincipal>();

            // mocking User Store
            var mockUserStore = new Mock<IUserStore<IdentityUser>>();

            var mockUser = new Mock<IdentityUser>();
            mockUser.Setup(u => u.Id).Returns(testId);

            mockUserStore
                .Setup(x => x.FindByIdAsync(testId, CancellationToken.None))
                .ReturnsAsync(mockUser.Object);

            var userManager = new UserManager<IdentityUser>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            userManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(mockUser.Object);

            // Mocking DbSet<UserProfile>
            var mockSet = new Mock<DbSet<UserProfile>>();
            mockSet.Setup(m => m.FindAsync(testId))
                .Returns(new ValueTask<UserProfile>(new UserProfile { UserId = testId }));

            // Mocking ProServDbContext
            var mockContext = new Mock<ProServDbContext>();
            mockContext.Setup(c => c.UserProfile).Returns(mockSet.Object);

            var controller = new UserController(new UserDbContextFactory(mockContext.Object), userManager)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = mockPrincipal.Object }
                }
            };

            // Act
            var result = await controller.GetUserProfile();
            Debug.WriteLine(result);

            // Assert
            Assert.IsNotNull(result);
            var actionResult = result.Result;
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            var userProfile = okResult.Value as UserProfile;
            Assert.IsNotNull(userProfile);
            Assert.AreEqual(testId, userProfile.UserId); // check if the returned profile's id is the same as the testId
        }



        [TestMethod()]
        public void PutUserProfileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostUserProfileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckIfUserInfoExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserInformationTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PutUserInformationTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UserInformationTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserTrackRecordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PutUserTrackRecordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostUserTrackRecordsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserProfileOnboardingTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PutProfileOnboardingTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostProfileOnboardingTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetProfileOnboardingCompletedTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CompleteProfileOnboardingTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserReportedInjuriesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PutReportedInjuriesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostReportedInjuriesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserGoalsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PutUserGoalsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostUserGoalsTest()
        {
            Assert.Fail();
        }
    }
}