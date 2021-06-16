using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MsaasBackend.Models;
using MsaasBackend.Tests.Utils;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class DepartmentsControllerTests : TestBase
    {
        public DepartmentsControllerTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDepartments_ValidIdentity_Success()
        {
            var res = await GetAs("/departments");
            res.EnsureSuccessStatusCode();
            var departments = await res.Content.ReadFromJsonAsync<ICollection<DepartmentDto>>();
            Assert.NotNull(departments);
            Assert.InRange(departments.Count, 1, 6);
        }

        [Fact]
        public async Task GetDepartmentById_ValidIdentity_Success()
        {
            var res = await GetAs("/departments/1");
            res.EnsureSuccessStatusCode();
            var departments = await res.Content.ReadFromJsonAsync<DepartmentDto>();
            Assert.Equal("department 1", departments?.Name);
        }

        [Fact]
        public async Task CreateDepartment_ValidIdentity_Success()
        {
            var form = new DepartmentCreationForm()
            {
                Name = "new depart",
                HospitalId = 1
            };

            var res = await PostJsonAs("/departments", form, Admin);
            res.EnsureSuccessStatusCode();
            var departments = await res.Content.ReadFromJsonAsync<DepartmentDto>();
            AssertExtensions.ContainsDeeply(form, departments);
        }

        [Fact]
        public async Task DeleteDepartment_ValidIdentity_Success()
        {
            var res = await DeleteAs("/departments/3", Admin);
            res.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateDepartment_ValidIdentity_Success()
        {
            var form = new DepartmentCreationForm()
            {
                Name = "updated departments",
            };

            var res = await PutJsonAs("/departments/4", form, Admin);
            res.EnsureSuccessStatusCode();
            var department = await res.Content.ReadFromJsonAsync<HospitalDto>();
            AssertExtensions.ContainsDeeply(form, department);
        }
    }
}