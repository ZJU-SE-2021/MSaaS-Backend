using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class GeneralTests : TestBase
    {
        public GeneralTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("GET", "/admin/users/1")]
        [InlineData("GET", "/admin/users")]
        [InlineData("DELETE", "/admin/users/1")]
        [InlineData("PUT", "/admin/users/1")]
        public async Task General_PrivilegedApi_Forbidden(string method, string uri)
        {
            var res = await SendAs(new HttpRequestMessage(new HttpMethod(method), uri), User);
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }
}