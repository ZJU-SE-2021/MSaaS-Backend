using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MsaasBackend.Models;
using MsaasBackend.Tests.Utils;
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
            await Login(User);
        }

        [Fact]
        public async Task GetCurrentUser_ValidIdentity_Success()
        {
            var res = await GetAs("/users/current");
            res.EnsureSuccessStatusCode();
            var user = await res.Content.ReadFromJsonAsync<UserDto>();
            Assert.Equal("user", user?.Username);
        }

        [Fact]
        public async Task GetUser_ValidIdentity_Success()
        {
            var res = await GetAs("/users/1", Admin);
            res.EnsureSuccessStatusCode();
            var user = await res.Content.ReadFromJsonAsync<UserDto>();
            Assert.Equal("user", user?.Username);
        }

        [Fact]
        public async Task GetUsers_ValidIdentity_Success()
        {
            var res = await GetAs("/users", Admin);
            res.EnsureSuccessStatusCode();
            var users = await res.Content.ReadFromJsonAsync<ICollection<UserDto>>();
            Assert.Equal(2, users?.Count);
        }

        [Fact]
        public async Task PutUsers_ValidForm_Success()
        {
            var registerForm = new RegisterForm
            {
                Username = "temp user",
                Password = "test password"
            };

            var regRes = await _client.PostAsJsonAsync("/users", registerForm);
            regRes.EnsureSuccessStatusCode();
            var user = await regRes.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(user);

            var form = new UpdateUserForm
            {
                Birthday = DateTime.Today,
                Email = "new@example.com",
                Gender = Gender.Other,
                // Password = "new password",
                Phone = "999-9999-9999",
                // Username = ""
            };

            var putRes = await PutJsonAs($"/users/{user.Id}", form, Admin);
            putRes.EnsureSuccessStatusCode();

            var getRes = await GetAs($"/users/{user.Id}", Admin);
            getRes.EnsureSuccessStatusCode();
            var updatedUser = await getRes.Content.ReadFromJsonAsync<UserDto>();

            AssertExtensions.ContainsDeeply(form, updatedUser);

            var deleteRes = await DeleteAs($"/users/{user.Id}", Admin);
            deleteRes.EnsureSuccessStatusCode();
        }
    }
}