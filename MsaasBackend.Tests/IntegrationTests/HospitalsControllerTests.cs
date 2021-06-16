using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MsaasBackend.Models;
using MsaasBackend.Tests.Utils;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class HospitalsControllerTests : TestBase
    {
        public HospitalsControllerTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetHospitals_ValidIdentity_Success()
        {
            var res = await GetAs("/hospitals");
            res.EnsureSuccessStatusCode();
            var hospitals = await res.Content.ReadFromJsonAsync<ICollection<HospitalDto>>();
            Assert.NotNull(hospitals);
            Assert.InRange(hospitals.Count, 1, 6);
        }

        [Fact]
        public async Task GetHospitalById_ValidIdentity_Success()
        {
            var res = await GetAs("/hospitals/1");
            res.EnsureSuccessStatusCode();
            var hospital = await res.Content.ReadFromJsonAsync<HospitalDto>();
            Assert.Equal("hospital 1", hospital?.Name);
        }

        [Fact]
        public async Task CreateHospital_ValidIdentity_Success()
        {
            var form = new HospitalCreationForm()
            {
                Name = "new hospital",
                Address = "new address"
            };

            var res = await PostJsonAs("/hospitals", form, Admin);
            res.EnsureSuccessStatusCode();
            var hospital = await res.Content.ReadFromJsonAsync<HospitalDto>();
            AssertExtensions.ContainsDeeply(form, hospital);
        }

        [Fact]
        public async Task DeleteHospital_ValidIdentity_Success()
        {
            var res = await DeleteAs("/hospitals/3", Admin);
            res.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateHospital_ValidIdentity_Success()
        {
            var form = new HospitalCreationForm()
            {
                Name = "updated hospital",
                Address = "updated address"
            };

            var res = await PutJsonAs("/hospitals/4", form, Admin);
            res.EnsureSuccessStatusCode();
            var hospital = await res.Content.ReadFromJsonAsync<HospitalDto>();
            AssertExtensions.ContainsDeeply(form, hospital);
        }
    }
}