using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Helpers;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("[controller]")]
    public class SummaryController : Controller
    {
        private readonly ILogger<SummaryController> _logger;

        public SummaryController(ILogger<SummaryController> logger, DataContext context) : base(context)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(SummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();
            var appointments =
                from a in _context.Appointments
                    .Include(a => a.User)
                    .Include(a => a.Physician)
                    .ThenInclude(p=>p.User)
                    .Include(a => a.Physician)
                    .ThenInclude(p => p.Department)
                    .ThenInclude(d => d.Hospital)
                    .Include(a => a.MedicalRecord)
                where a.UserId == userId &&
                      a.Time > DateTime.Now
                orderby a.Time
                select a;
            var appointment = await appointments.FirstOrDefaultAsync();
            var records =
                from m in _context.MedicalRecords
                    .Include(m => m.Appointment)
                where m.Appointment.UserId == userId
                orderby m.Appointment.Time descending
                select m;
            var record = await records.FirstOrDefaultAsync();
            return Ok(new SummaryDto()
            {
                RecentAppointment = appointment?.ToDto(),
                RecentMedicalRecord = record?.ToDto()
            });
        }
    }
}