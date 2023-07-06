using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProServ.Server.Contexts;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ProServ.Server.IntegrationTests
{
    public class TeamControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public TeamControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAllTeamPackagesAsync_ReturnsData()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Replace the DbContext registration with an in-memory database
                    services.AddDbContext<ProServDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/api/info/all-packages");

            // Assert
            response.EnsureSuccessStatusCode();
            // Add additional assertions based on the expected response
        }

        [Fact]
        public async Task TeamNameExistsAsync_ReturnsFalse_WhenTeamDoesNotExist()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Replace the DbContext registration with an in-memory database
                    services.AddDbContext<ProServDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/api/info/team-name-exists?teamName=NonExistingTeam");

            // Assert
            response.EnsureSuccessStatusCode();
            // Add additional assertions based on the expected response
        }
    }
}
