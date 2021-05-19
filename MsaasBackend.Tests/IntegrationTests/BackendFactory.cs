using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;
using BC = BCrypt.Net.BCrypt;

namespace MsaasBackend.Tests.IntegrationTests
{
    public class BackendFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private const string _dbName = "MsaasTestDatabase";

        private void InitDatabase(DataContext db)
        {
            var user = new User()
            {
                Username = "user",
                PasswordHash = BC.EnhancedHashPassword("user password"),
            };
            var admin = new User()
            {
                Username = "admin",
                PasswordHash = BC.EnhancedHashPassword("admin password"),
                Role = "Admin"
            };
            db.Users.AddRange(user, admin);
            db.SaveChanges();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<DataContext>));

                services.Remove(descriptor);

                services.AddDbContext<DataContext>(options => { options.UseInMemoryDatabase(_dbName); });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DataContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<BackendFactory<TStartup>>>();

                    db.Database.EnsureCreated();
                    InitDatabase(db);
                }
            });
        }
    }
}