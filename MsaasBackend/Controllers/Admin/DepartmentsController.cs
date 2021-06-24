using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers.Admin
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("Admin/[controller]")]
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            if (department.Name != form.Name)
            {
                var departments = from d in _context.Departments where d.Name == form.Name select d;
                if (await departments.AnyAsync()) return Conflict();
                department.Name = form.Name;
            }

            department.Section = form.Section;

            await _context.SaveChangesAsync();

            await _context.Entry(department).Reference(d => d.Hospital).LoadAsync();

            return Ok(department.ToDto());
        }

        [HttpPost]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateDepartment(DepartmentCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var hospital = await _context.Hospitals.FindAsync(form.HospitalId);
            if (hospital == null) return NotFound();

            var departments =
                from d in _context.Departments
                where d.HospitalId == form.HospitalId && d.Name == form.Name
                select d;
            if (await departments.AnyAsync()) return Conflict();

            var department = new Department
            {
                Name = form.Name,
                HospitalId = form.HospitalId,
                Section = form.Section
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            await _context.Entry(department).Reference(d => d.Hospital).LoadAsync();

            return CreatedAtAction("GetDepartment", new {Id = department.Id}, department.ToDto());
        }
    }
}