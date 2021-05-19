using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MsaasBackend.Models;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<BackendFactory<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class TestBase// : IClassFixture<BackendFactory<Startup>>
    {
        protected readonly BackendFactory<Startup> _factory;
        protected readonly HttpClient _client;
        private string _userToken = null;
        private string _adminToken = null;

        public TestBase(BackendFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        protected async Task<LoginResult> Login(string username, string password)
        {
            var client = _factory.CreateClient();
            var form = new LoginForm()
            {
                Username = username,
                Password = password
            };
            var res = await client.PostAsJsonAsync("users/login", form);
            res.EnsureSuccessStatusCode();
            var data = await res.Content.ReadFromJsonAsync<LoginResult>();
            Assert.NotNull(data?.Token);
            Assert.NotNull(data.User);
            return data;
        }

        protected async Task<string> GetUserToken()
        {
            if (_userToken == null)
            {
                _userToken = (await Login("user", "user password")).Token;
            }

            return _userToken;
        }

        protected async Task<string> GetAdminToken()
        {
            if (_adminToken == null)
            {
                _adminToken = (await Login("admin", "admin password")).Token;
            }

            return _adminToken;
        }

        protected async Task<HttpResponseMessage> SendAsUser(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetUserToken());
            return await _client.SendAsync(request);
        }

        protected async Task<HttpResponseMessage> SendAsAdmin(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminToken());
            return await _client.SendAsync(request);
        }

        protected async Task<HttpResponseMessage> GetAsUser(string requestUri)
        {
            return await SendAsUser(new HttpRequestMessage(HttpMethod.Get, requestUri));
        }

        protected async Task<HttpResponseMessage> GetAsAdmin(string requestUri)
        {
            return await SendAsAdmin(new HttpRequestMessage(HttpMethod.Get, requestUri));
        }
    }
}