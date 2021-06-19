using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class PhysiciansController : ControllerBase
    {
        private readonly ILogger<PhysiciansController> _logger;
        private readonly DataContext _context;

        public PhysiciansController(ILogger<PhysiciansController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PhysicianDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhysicians(int? departmentId)
        {
            var res =
                from p in _context.Physicians
                where !departmentId.HasValue || p.DepartmentId == departmentId
                select p.ToDto();
            return Ok(await res.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PhysicianDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhysicianById(int id)
        {
            var physicians = from u in _context.Physicians where u.Id == id select u;
            var physician = await physicians.FirstOrDefaultAsync();
            if (physician == null) return NotFound();
            return Ok(physician.ToDto());
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhysician(int id)
        {
            var physician = await _context.Physicians.FindAsync(id);
            if (physician == null) return NotFound();
            _context.Physicians.Remove(physician);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(PhysicianDto), StatusCodes.Status201Created)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterPhysician(PhysicianRegisterForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var departments = from d in _context.Departments where d.Id == form.DepartmentId select d;
            var department = await departments.FirstOrDefaultAsync();
            if (department == null) return NotFound();

            var user = await _context.Users.FindAsync(form.UserId);
            if (user == null) return NotFound();

            user.Role = "Physician";
            var physician = new Physician()
            {
                DepartmentId = form.DepartmentId,
                UserId = form.UserId
            };
            _context.Physicians.Add(physician);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPhysicianById), new {Id = physician.Id}, physician.ToDto());
        }
    }
}