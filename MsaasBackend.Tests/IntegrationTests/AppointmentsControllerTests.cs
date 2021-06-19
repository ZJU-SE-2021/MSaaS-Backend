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
    public class AppointmentsControllerTests : TestBase
    {
        public AppointmentsControllerTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAppointmentById_ValidIdentity_Success()
        {
            var res = await GetAs("/appointments/1");
            res.EnsureSuccessStatusCode();
            var appointment = await res.Content.ReadFromJsonAsync<AppointmentDto>();
            Assert.Equal(1, appointment?.UserId);
        }

        [Fact]
        public async Task AddAppointment_ValidIdentity_Success()
        {
            var form = new AppointmentForm
            {
                PhysicianId = 1,
                Time = DateTime.Now
            };

            var res = await PostJsonAs("/appointments", form);
            res.EnsureSuccessStatusCode();
            var appointment = await res.Content.ReadFromJsonAsync<AppointmentDto>();
            AssertExtensions.ContainsDeeply(form, appointment);
        }
    }
}