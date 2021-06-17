using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MsaasBackend.Models;
using MsaasBackend.Tests.Utils;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class PhysiciansControllerTests : TestBase
    {
        public PhysiciansControllerTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetPhysicians_ValidIdentity_Success()
        {
            var res = await GetAs("/physicians");
            res.EnsureSuccessStatusCode();
            var physician = await res.Content.ReadFromJsonAsync<ICollection<PhysicianDto>>();
            Assert.NotNull(physician);
            Assert.InRange(physician.Count, 1, 7);
        }

        [Fact]
        public async Task GetPhysicianById_ValidIdentity_Success()
        {
            var res = await GetAs("/physicians/3");
            res.EnsureSuccessStatusCode();
            var physician = await res.Content.ReadFromJsonAsync<PhysicianDto>();
            Assert.Equal(3, physician?.UserId);
        }

        [Fact]
        public async Task RegisterPhysician_ValidIdentity_Success()
        {
            var form = new PhysicianRegisterForm()
            {
                UserId = 7,
                DepartmentId = 1
            };

            var res = await PostJsonAs("/physicians", form, Admin);
            res.EnsureSuccessStatusCode();
            var physician = await res.Content.ReadFromJsonAsync<PhysicianDto>();
            AssertExtensions.ContainsDeeply(form, physician);
        }

        [Fact]
        public async Task DeletePhysician_ValidIdentity_Success()
        {
            var res = await DeleteAs("/physicians/7", Admin);
            res.EnsureSuccessStatusCode();
        }
    }
}