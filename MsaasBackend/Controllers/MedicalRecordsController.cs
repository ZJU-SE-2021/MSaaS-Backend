using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MsaasBackend.Helpers;

namespace MsaasBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme)]
    public class MedicalRecordsController : Controller
    {
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(ILogger<MedicalRecordsController> logger, DataContext context) : base(context)
        {
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();

            var records =
                from r in _context.MedicalRecords
                    .Include(r => r.Appointment)
                where r.Id == id && r.Appointment.UserId == userId
                select r;
            var record = await records.FirstOrDefaultAsync();
            if (record == null) return NotFound();

            return Ok(record.ToDto());
        }
    }
}