using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MsaasBackend.Models;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class UsersControllerTests : TestBase
    {
        public UsersControllerTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Login_ValidIdentity_Success()
        {
            await GetUserToken();
        }

        [Fact]
        public async Task GetCurrentUser_ValidIdentity_Success()
        {
            var res = await GetAsUser("/users/current");
            res.EnsureSuccessStatusCode();
            var user = await res.Content.ReadFromJsonAsync<UserDto>();
            Assert.Equal("user", user?.Username);
        }

        [Fact]
        public async Task GetUser_ValidIdentity_Success()
        {
            var res = await GetAsAdmin("/users/1");
            res.EnsureSuccessStatusCode();
            var user = await res.Content.ReadFromJsonAsync<UserDto>();
            Assert.Equal("user", user?.Username);
        }

        [Fact]
        public async Task GetUsers_ValidIdentity_Success()
        {
            var res = await GetAsAdmin("/users");
            res.EnsureSuccessStatusCode();
            var users = await res.Content.ReadFromJsonAsync<ICollection<UserDto>>();
            Assert.Equal(2, users?.Count);
        }
    }
}