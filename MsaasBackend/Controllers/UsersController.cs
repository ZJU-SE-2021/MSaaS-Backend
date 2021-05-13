using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MsaasBackend.Models;
using BC = BCrypt.Net.BCrypt;

namespace MsaasBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly DataContext _context;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public UsersController(ILogger<UsersController> logger, DataContext context,
            IOptions<JwtOptions> jwtOptions)
        {
            _logger = logger;
            _context = context;
            _jwtOptions = jwtOptions;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var users = from u in _context.Users where u.Username == form.Username select u;
            var user = await users.FirstOrDefaultAsync();

            if (user == null || !BC.EnhancedVerify(form.Password, user.PasswordHash))
                return Unauthorized();

            // sign the JWT token
            var handler = new JwtSecurityTokenHandler();
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddSeconds(_jwtOptions.Value.ExpiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtOptions.Value.SigningKeyData),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handler.CreateToken(tokenDesc);
            return Ok(new
            {
                Token = handler.WriteToken(token),
                User = user.ToDto()
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
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
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new {Id = user.Id}, user.ToDto());
        }

        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentId == null) return Unauthorized();
            return await GetUser(Convert.ToInt32(currentId.Value));
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var users = from u in _context.Users where u.Id == id select u;
            var user = await users.FirstOrDefaultAsync();
            if (user == null) return NotFound();
            return Ok(user.ToDto());
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = from u in _context.Users select u.ToDto();
            return Ok(users);
        }
    }
}