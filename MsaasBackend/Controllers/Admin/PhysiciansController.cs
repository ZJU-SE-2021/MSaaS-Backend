using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Helpers;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers.Admin
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("Admin/[controller]")]
    [ApiController]
    public class PhysiciansController : Controller
    {
        private readonly ILogger<PhysiciansController> _logger;

        public PhysiciansController(ILogger<PhysiciansController> logger, DataContext context) : base(context)
        {
            _logger = logger;
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
            await _context.Entry(physician)
                .Reference(p => p.Department)
                .Query()
                .Include(d=>d.Hospital)
                .LoadAsync();

            return CreatedAtAction("GetPhysicianById", new {Id = physician.Id}, physician.ToDto());
        }
    }
}