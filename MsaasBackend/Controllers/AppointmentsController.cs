using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly DataContext _context;

        public AppointmentsController(ILogger<AppointmentsController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();
            var userId = Convert.ToInt32(claim.Value);
            if (appointment.UserId != userId && appointment.PhysicianId != userId) return Forbid();

            return Ok(appointment.ToDto());
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

            var physicians = from u in _context.Physicians where u.UserId == form.PhysicianId select u;
            var physician = await physicians.FirstOrDefaultAsync();
            if (physician == null) return NotFound();

            var appointment = new Appointment
            {
                UserId = user.Id,
                User = user,
                PhysicianId = form.PhysicianId,
                Physician = physician,
                Description = form.Description
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAppointmentById), new {Id = appointment.Id}, appointment.ToDto());
        }
    }
}