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
    [ApiController]
    [Route("[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly ILogger<HospitalsController> _logger;
        private readonly DataContext _context;

        public HospitalsController(ILogger<HospitalsController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHospitals()
        {
            var hospitals = from h in _context.Hospitals select h.ToDto();
            return Ok(await hospitals.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHospital(int id)
        {
            var hospital = from h in _context.Hospitals where h.Id == id select h.ToDto();
            return Ok(await hospital.ToListAsync());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHospital(HospitalCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var hospitals = from h in _context.Hospitals where h.Name == form.Name select h;
            if (await hospitals.AnyAsync()) return Conflict();

            var hospital = new Hospital
            {
                Name = form.Name,
                Address = form.Address,
            };

            _context.Hospitals.Add(hospital);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHospital), new { Id = hospital.Id }, hospital.ToDto());
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHospital(int id, HospitalCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var hospital = await _context.Hospitals.FindAsync(id);
            return await UpdateHospital(hospital, form);
        }

        public async Task<IActionResult> UpdateHospital(Hospital hospital, HospitalCreationForm form)
        {
            if (hospital.Name != form.Name)
            {
                var hospitals = from h in _context.Hospitals where h.Name == form.Name select h;
                if (await hospitals.AnyAsync()) return Conflict();
                hospital.Name = form.Name;
            }
            hospital.Address = form.Address;
            await _context.SaveChangesAsync();
            return Ok(hospital);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null) return NotFound();
            _context.Hospitals.Remove(hospital);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id:int}/Departments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDepartments(int id)
        {
            var departments = from d in _context.Departments select d.ToDto();
            return Ok(await departments.ToListAsync());
        }

        [HttpPost("{id:int}/Departments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDepartment(int id, DepartmentCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var hospital = (from h in _context.Hospitals where h.Id == id select h).Single();
            var departments = from d in _context.Departments where d.HospitalId == id select d;
            if (await departments.AnyAsync()) return Conflict();

            var department = new Department
            {
                Id = form.GetHashCode(),
                Name = form.Name,
                Hospital = hospital,
                HospitalId = id
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartments), new { Id = department.Id }, department.ToDto());
        }
    }
}