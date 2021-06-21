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
    [Route("[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly DataContext _context;

        public DepartmentsController(ILogger<DepartmentsController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartments(int? hospitalId)
        {
            var departments =
                from d in _context.Departments
                where !hospitalId.HasValue || d.HospitalId == hospitalId
                select d.ToDto();
            return Ok(await departments.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            return Ok(department.ToDto());
        }
    }
}