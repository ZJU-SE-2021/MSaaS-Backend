using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MsaasBackend.Controllers
{
    public static class AuthenticationDefaults
    {
        public const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme + "," +
                                                   JwtBearerDefaults.AuthenticationScheme;
    }
}