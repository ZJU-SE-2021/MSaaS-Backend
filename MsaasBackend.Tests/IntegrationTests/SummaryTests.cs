using System.Net.Http.Json;
using System.Threading.Tasks;
using MsaasBackend.Models;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class SummaryControllerTests : TestBase
    {
        public SummaryControllerTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetSummary_ValidIdentity_Success()
        {
            var res = await GetAs("/summary");
            res.EnsureSuccessStatusCode();
            var summary = await res.Content.ReadFromJsonAsync<SummaryDto>();
            Assert.NotNull(summary);
        }
    }
}