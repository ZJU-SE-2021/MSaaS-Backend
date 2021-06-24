using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme)]
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
                    .Include(p => p.Department)
                    .ThenInclude(d=>d.Hospital)
                    .Include(p => p.User)
                where !departmentId.HasValue || p.DepartmentId == departmentId
                select p.ToDto();
            return Ok(await res.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PhysicianDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhysicianById(int id)
        {
            var physicians =
                from u in _context.Physicians
                    .Include(u => u.User)
                    .Include(u => u.Department)
                    .ThenInclude(d=>d.Hospital)
                where u.Id == id
                select u;
            var physician = await physicians.FirstOrDefaultAsync();
            if (physician == null) return NotFound();
            return Ok(physician.ToDto());
        }
    }
}