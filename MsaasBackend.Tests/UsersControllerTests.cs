using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MsaasBackend.Controllers;
using MsaasBackend.Models;
using Xunit;
using BC = BCrypt.Net.BCrypt;

namespace MsaasBackend.Tests
{
    public class UsersControllerTests : InMemoryDataContextTests
    {
        private UsersController UsersController { get; set; }

        public UsersControllerTests()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<UsersController>();

            var jwtOptions = new JwtOptions() {ExpiresIn = 60 * 60, SigningKey = "test signing key"};
            var options = Options.Create<JwtOptions>(jwtOptions);
            UsersController = new UsersController(logger, DataContext, options);

            Seed();
        }

        private void Seed()
        {
            var normal = new User()
            {
                Username = "normal",
                PasswordHash = BC.EnhancedHashPassword("normal pass"),
            };

            var admin = new User()
            {
                Username = "admin",
                PasswordHash = BC.EnhancedHashPassword("admin pass"),
            };

            DataContext.Users.AddRange(normal, admin);
            DataContext.SaveChanges();
        }

        [Fact]
        public async Task Register_ValidForm_Success()
        {
            var form = new RegisterForm()
            {
                Username = "new user",
                Password = "user pass",
                Birthday = new DateTime(2020, 1, 1),
                Email = "test@example.com",
                Gender = Gender.Male,
                Phone = "123-1234-1234"
            };
            var res = await UsersController.CreateUser(form);
            Assert.IsType<CreatedAtActionResult>(res);

            var loginRes = await Login("new user", "user pass");
            var user = loginRes.User;
            foreach (var p in user.GetType().GetProperties())
            {
                var expectedProp = form.GetType().GetProperty(p.Name);
                if (expectedProp == null) continue;
                Assert.Equal(expectedProp.GetValue(form), p.GetValue(user));
            }
        }

        [Fact]
        public async Task Login_ValidIdentity_Success()
        {
            var res = await Login("normal", "normal pass");
            Assert.Equal("normal", res.User?.Username);
        }

        private async Task<LoginResult> Login(string username, string password)
        {
            var form = new LoginForm {Username = username, Password = password};
            var res = await UsersController.Login(form);
            var okRes = Assert.IsType<OkObjectResult>(res);
            var data = Assert.IsType<LoginResult>(okRes.Value);
            Assert.NotNull(data.Token);
            Assert.NotNull(data.User);
            return data;
        }
    }
}