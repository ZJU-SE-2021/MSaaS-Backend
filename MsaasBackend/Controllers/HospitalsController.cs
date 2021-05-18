using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetHospitals()
        {
            var hospitals = from h in _context.Hospitals select h.toDto();
            return Ok(hospitals);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHospital(HospitalCreationForm form)
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        public async Task<IActionResult> ModifyHospital(HospitalCreationForm form)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            throw new NotImplementedException();
        }
    }
}