using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Route("[controller]")]
    [ApiController]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(ILogger<AppointmentsController> logger, DataContext context) : base(context)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAppointments()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();
            var res =
                from a in _context.Appointments
                    .Include(a => a.MedicalRecord)
                    .Include(a => a.User)
                    .Include(a => a.Physician)
                    .ThenInclude(p => p.Department)
                    .ThenInclude(d=>d.Hospital)
                    .Include(a => a.Physician)
                    .ThenInclude(p => p.User)
                where a.UserId == userId
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
                    .Include(a => a.MedicalRecord)
                    .Include(a => a.User)
                    .Include(a => a.Physician)
                    .ThenInclude(p => p.Department)
                    .ThenInclude(d=>d.Hospital)
                    .Include(a => a.Physician)
                    .ThenInclude(p => p.User)
                where a.Id == id && a.UserId == userId
                select a.ToDto();
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAppointment(AppointmentForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentId == null) return Unauthorized();

            var users = from u in _context.Users where u.Id == Convert.ToInt32(currentId.Value) select u;
            var user = await users.FirstOrDefaultAsync();
            if (user == null) return NotFound();

            var physicians = from u in _context.Physicians where u.Id == form.PhysicianId select u;
            var physician = await physicians.FirstOrDefaultAsync();
            if (physician == null) return NotFound();

            var appointment = new Appointment
            {
                UserId = user.Id,
                PhysicianId = form.PhysicianId,
                Description = form.Description,
                Time = form.Time
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            await _context.Entry(appointment).Reference(a => a.User).LoadAsync();
            await _context.Entry(appointment)
                .Reference(a => a.Physician)
                .Query()
                .Include(p => p.Department)
                .ThenInclude(d=>d.Hospital)
                .Include(p => p.User)
                .LoadAsync();
            return CreatedAtAction(nameof(GetAppointmentById), new {Id = appointment.Id}, appointment.ToDto());
        }
    }
}