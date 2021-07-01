using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Helpers;
using MsaasBackend.Models;
using BC = BCrypt.Net.BCrypt;

namespace MsaasBackend.Controllers.Admin
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("Admin/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, DataContext context) : base(context)
        {
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUser(int id)
        {
            var users = from u in _context.Users where u.Id == id select u;
            var user = await users.FirstOrDefaultAsync();
            if (user == null) return NotFound();
            return Ok(user.ToDto());
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers()
        {
            var users = from u in _context.Users select u.ToDto();
            return Ok(await users.ToListAsync());
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserFormAdmin form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var user = await _context.Users.FindAsync(id);

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
            user.Role = form.Role;

            if (form.Password != null) user.PasswordHash = BC.EnhancedHashPassword(form.Password);
            await _context.SaveChangesAsync();

            return Ok(user.ToDto());
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}