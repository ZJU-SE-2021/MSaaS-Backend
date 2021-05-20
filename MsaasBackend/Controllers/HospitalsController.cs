using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Msaasbackend.Controllers;
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
        public async Task<IActionResult> GetHospitals()
        {
            var hospitals = from h in _context.Hospitals select h.ToDto();
            return Ok(await hospitals.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null) return NotFound();
            return Ok(hospital.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateHospital(HospitalCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var hospitals = from h in _context.Hospitals where h.Name == form.Name select h;
            if (await hospitals.AnyAsync()) return Conflict();

            var hospital = new Hospital
            {
                Name = form.Name,
                // Departments = new List<Department>(),
                Address = form.Address,
            };

            _context.Hospitals.Add(hospital);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHospital), new { Id = hospital.Id }, hospital.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateHospital(int id, HospitalCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var hospital = await _context.Hospitals.FindAsync(id);

            if (hospital.Name != form.Name)
            {
                var hospitals = from h in _context.Hospitals where h.Name == form.Name select h;
                if (await hospitals.AnyAsync()) return Conflict();
                hospital.Name = form.Name;
            }

            hospital.Address = form.Address;
            await _context.SaveChangesAsync();
            return Ok(hospital.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null) return NotFound();

            var departments = from d in _context.Departments where d.HospitalId == id select d;

            _context.Hospitals.Remove(hospital);
            foreach (var department in departments)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}