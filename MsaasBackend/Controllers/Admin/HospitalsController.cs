using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MsaasBackend.Helpers;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers.Admin
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("Admin/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly ILogger<HospitalsController> _logger;
        private readonly DataContext _context;
        private readonly IDistributedCache _distributedCache;

        public HospitalsController(ILogger<HospitalsController> logger, DataContext context,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _context = context;
            _distributedCache = distributedCache;
        }

        [HttpPost]
        [ProducesResponseType(typeof(HospitalDto), StatusCodes.Status201Created)]
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
            // Invalidate cache
            await _distributedCache.RemoveAsync(Constants.CacheKey.GetHospitalsCacheKey);
            return CreatedAtAction("GetHospital", new {Id = hospital.Id}, hospital.ToDto());
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(HospitalDto), StatusCodes.Status200OK)]
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
            // Invalidate cache
            await _distributedCache.RemoveAsync(Constants.CacheKey.GetHospitalsCacheKey);
            await _distributedCache.RemoveAsync(Constants.CacheKey.GetHospitalCacheKey(hospital.Id));
            return Ok(hospital.ToDto());
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
            // Invalidate cache
            await _distributedCache.RemoveAsync(Constants.CacheKey.GetHospitalsCacheKey);
            await _distributedCache.RemoveAsync(Constants.CacheKey.GetHospitalCacheKey(hospital.Id));
            return Ok();
        }
    }
}