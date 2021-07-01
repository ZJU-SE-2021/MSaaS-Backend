using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MsaasBackend.Helpers;
using MsaasBackend.Models;
using BC = BCrypt.Net.BCrypt;

namespace MsaasBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public UsersController(ILogger<UsersController> logger, DataContext context,
            IOptions<JwtOptions> jwtOptions) : base(context)
        {
            _logger = logger;
            _jwtOptions = jwtOptions;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var users = from u in _context.Users where u.Username == form.Username select u;
            var user = await users.FirstOrDefaultAsync();

            if (user == null || !BC.EnhancedVerify(form.Password, user.PasswordHash))
                return Unauthorized();

            // prepare claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            // sign the JWT token
            var handler = new JwtSecurityTokenHandler();
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(_jwtOptions.Value.ExpiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtOptions.Value.SigningKeyData),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handler.CreateToken(tokenDesc);

            // dispatch the cookies
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaims(claims);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(_jwtOptions.Value.ExpiresIn)
                });

            return Ok(new LoginResult
            {
                Token = handler.WriteToken(token),
                User = user.ToDto()
            });
        }

        [HttpGet("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser(RegisterForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var users = from u in _context.Users where u.Username == form.Username select u;
            if (await users.AnyAsync()) return Conflict();

            var user = new User
            {
                Username = form.Username,
                PasswordHash = BC.EnhancedHashPassword(form.Password),
                Name = form.Name,
                Birthday = form.Birthday,
                Email = form.Email,
                Gender = form.Gender,
                Phone = form.Phone
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCurrentUser), user.ToDto());
        }

        [HttpGet("Current")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();

            var users = from u in _context.Users where u.Id == userId select u;
            var user = await users.FirstOrDefaultAsync();
            if (user == null) return NotFound();
            return Ok(user.ToDto());
        }

        [HttpPut("Current")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateCurrentUser(UpdateUserForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var user = await GetUser();

            if (user.Username != form.Username)
            {
                var users = from u in _context.Users where u.Username == form.Username select u;
                if (await users.AnyAsync()) return Conflict();
                user.Username = form.Username;
            }

            user.Name = form.Name;
            user.Birthday = form.Birthday;
            user.Email = form.Email;
            user.Gender = form.Gender;
            user.Phone = form.Phone;

            if (form.Password != null) user.PasswordHash = BC.EnhancedHashPassword(form.Password);
            await _context.SaveChangesAsync();

            return Ok(user.ToDto());
        }
    }
}