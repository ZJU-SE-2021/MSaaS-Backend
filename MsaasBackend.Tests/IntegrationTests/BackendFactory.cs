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

            var hospital1 = new Hospital()
            {
                Name = "hospital 1"
            };

            var hospital2 = new Hospital()
            {
                Name = "hospital 2"
            };

            var hospital3 = new Hospital()
            {
                Name = "hospital 3"
            };

            var hospital4 = new Hospital()
            {
                Name = "hospital 4"
            };

            db.Hospitals.AddRange(hospital1, hospital2, hospital3, hospital4);
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