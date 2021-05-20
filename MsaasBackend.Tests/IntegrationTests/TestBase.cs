using System.Collections.Generic;
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

    public class TestBase // : IClassFixture<BackendFactory<Startup>>
    {
        protected readonly BackendFactory<Startup> _factory;
        protected readonly HttpClient _client;

        protected static LoginForm Admin { get; } = new() {Username = "admin", Password = "admin password"};
        protected static LoginForm User { get; } = new() {Username = "user", Password = "user password"};

        private Dictionary<string, string> _tokenCache = new();

        public TestBase(BackendFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        protected async Task<LoginResult> Login(LoginForm form)
        {
            var client = _factory.CreateClient();
            var res = await client.PostAsJsonAsync("users/login", form);
            res.EnsureSuccessStatusCode();
            var data = await res.Content.ReadFromJsonAsync<LoginResult>();
            Assert.NotNull(data?.Token);
            Assert.NotNull(data.User);
            return data;
        }

        protected async Task<string> GetToken(LoginForm form, bool cached = true)
        {
            if (!cached || !_tokenCache.TryGetValue(form.Username, out var token))
            {
                token = (await Login(form)).Token;
                _tokenCache[form.Username] = token;
            }

            return token;
        }

        protected async Task<HttpResponseMessage> SendAs(HttpRequestMessage request, LoginForm user = null)
        {
            user ??= User;
            var token = await GetToken(user);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _client.SendAsync(request);
        }

        protected async Task<HttpResponseMessage> SendAs(HttpMethod method, string uri, HttpContent content,
            LoginForm user = null)
        {
            return await SendAs(new HttpRequestMessage(method, uri) {Content = content}, user);
        }

        protected async Task<HttpResponseMessage> SendJsonAs<TValue>(HttpMethod method, string uri, TValue value,
            LoginForm user = null)
        {
            var jsonContent = JsonContent.Create(value);
            return await SendAs(method, uri, jsonContent, user);
        }

        protected async Task<HttpResponseMessage> GetAs(string requestUri, LoginForm user = null)
        {
            return await SendAs(HttpMethod.Get, requestUri, null, user);
        }

        protected async Task<HttpResponseMessage> PostAs(string requestUri, HttpContent content, LoginForm user = null)
        {
            return await SendAs(HttpMethod.Post, requestUri, content, user);
        }

        protected async Task<HttpResponseMessage> PostJsonAs<TValue>(string requestUri, TValue value,
            LoginForm user = null)
        {
            return await SendJsonAs(HttpMethod.Post, requestUri, value, user);
        }

        protected async Task<HttpResponseMessage> PutAs(string requestUri, HttpContent content, LoginForm user = null)
        {
            return await SendAs(HttpMethod.Put, requestUri, content, user);
        }

        protected async Task<HttpResponseMessage> PutJsonAs<TValue>(string requestUri, TValue value,
            LoginForm user = null)
        {
            return await SendJsonAs(HttpMethod.Put, requestUri, value, user);
        }

        protected async Task<HttpResponseMessage> DeleteAs(string requestUri, LoginForm user = null)
        {
            return await SendAs(HttpMethod.Delete, requestUri, null, user);
        }

        protected async Task<HttpResponseMessage> PatchAs(string requestUri, HttpContent content, LoginForm user = null)
        {
            return await SendAs(HttpMethod.Patch, requestUri, content, user);
        }

        protected async Task<HttpResponseMessage> PatchJsonAs<TValue>(string requestUri, TValue value,
            LoginForm user = null)
        {
            return await SendJsonAs(HttpMethod.Patch, requestUri, value, user);
        }
    }
}