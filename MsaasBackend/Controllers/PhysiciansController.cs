using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
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
        public async Task<IActionResult> GetPhysicians()
        {
            return Ok(await _context.Physicians.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PhysicianDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhysicianById(int id)
        {
            var physicians = from u in _context.Physicians where u.UserId == id select u;
            var physician = await physicians.FirstOrDefaultAsync();
            if (physician == null) return NotFound();
            return Ok(physician.ToDto());
        }
        
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

            return CreatedAtAction(nameof(GetPhysicianById), new {Id = form.UserId}, physician.ToDto());
        }
    }
}