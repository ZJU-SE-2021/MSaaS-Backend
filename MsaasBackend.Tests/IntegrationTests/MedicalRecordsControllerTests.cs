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
    public class MedicalRecordsControllerTests : TestBase
    {
        public MedicalRecordsControllerTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetMedicalRecordById_ValidIdentity_Success()
        {
            var res = await GetAs("/medicalrecords/1");
            res.EnsureSuccessStatusCode();
            var medicalRecord = await res.Content.ReadFromJsonAsync<MedicalRecordDto>();
            Assert.Equal(1, medicalRecord?.AppointmentId);
        }

        [Fact]
        public async Task AddMedicalRecord_ValidIdentity_Success()
        {
            var form = new MedicalRecordForm()
            {
                AppointmentId = 2,
                Diagnosis = "Diagnosis",
                PastMedicalHistory = "PastMedicalHistory",
                Prescription = "Prescription",
                Symptom = "Symptom"
            };

            var res = await PostJsonAs("/physicians/medicalrecords", form, Physician);
            res.EnsureSuccessStatusCode();
            var medicalRecord = await res.Content.ReadFromJsonAsync<MedicalRecordDto>();
            AssertExtensions.ContainsDeeply(form, medicalRecord);
        }
    }
}