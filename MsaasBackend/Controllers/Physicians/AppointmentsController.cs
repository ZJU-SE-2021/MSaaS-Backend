using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers.Physicians
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme, Roles = "Physician")]
    [Route("Physicians/[controller]")]
    [ApiController]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly DataContext _context;

        public AppointmentsController(ILogger<AppointmentsController> logger, DataContext context) : base(context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAppointments()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();
            var res =
                from a in _context.Appointments
                where a.PhysicianId == userId
                select a.ToDto();
            return Ok(await res.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();
            var appointments =
                from a in _context.Appointments
                where a.Id == id && a.PhysicianId == userId
                select a.ToDto();
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }
    }
}