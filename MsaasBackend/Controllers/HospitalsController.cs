using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _distributedCache;

        public HospitalsController(ILogger<HospitalsController> logger, DataContext context, IDistributedCache distributedCache)
        {
            _logger = logger;
            _context = context;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HospitalDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitals()
        {
            var cachedHospitals = await _distributedCache.GetStringAsync("Hospitals");
            if (cachedHospitals != null && cachedHospitals != "[]")
            {
                return Ok(JsonSerializer.Deserialize<HospitalDto[]>(cachedHospitals));
            }
            var hospitals = from h in _context.Hospitals select h.ToDto();
            var hospitalsList = await hospitals.ToListAsync();
            var serializedString = JsonSerializer.Serialize(hospitalsList);
            var option =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(2));
            await _distributedCache.SetStringAsync("Hospitals", serializedString, option);

            return Ok(hospitalsList);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(HospitalDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospital(int id)
        {
            var cachedHospital = await _distributedCache.GetStringAsync($"Hospitals/{id}");
            if (cachedHospital != null && cachedHospital != "{}")
            {
                return Ok(JsonSerializer.Deserialize<HospitalDto>(cachedHospital));
            }
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null) return NotFound();
            var hospitalDto = hospital.ToDto();
            var serializedString = JsonSerializer.Serialize(hospitalDto);
            var option =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(2));
            await _distributedCache.SetStringAsync($"Hospitals/{id}", serializedString, option);
            return Ok(hospitalDto);
        }
    }
}