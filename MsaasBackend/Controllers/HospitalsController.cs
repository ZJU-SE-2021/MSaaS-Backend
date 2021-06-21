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
        [ProducesResponseType(typeof(IEnumerable<HospitalDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitals()
        {
            var hospitals = from h in _context.Hospitals select h.ToDto();
            return Ok(await hospitals.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(HospitalDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null) return NotFound();
            return Ok(hospital.ToDto());
        }
    }
}