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

        [HttpPost]
        [ProducesResponseType(typeof(Physician), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterPhysician(int id, PhysicianRegisterForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var departments = from d in _context.Departments where d.Id == id select d;
            var department = await departments.FirstOrDefaultAsync();
            if (department == null) return NotFound();

            var physicians = from u in _context.Physicians where u.Id == form.PhysicianId select u;
            var physician = await physicians.FirstOrDefaultAsync();
            if (physician == null) return NotFound();

            physician.DepartmentId = id;
            physician.Department = department;

            return Ok(physician);
        }
    }
}