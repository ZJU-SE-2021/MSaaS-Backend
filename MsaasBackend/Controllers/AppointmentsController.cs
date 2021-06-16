using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    [Authorize]
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

        [HttpPost]
        [ProducesResponseType(typeof(Appointment), StatusCodes.Status201Created)]
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
                User = user,
                PhysicianId = form.PhysicianId,
                Physician = physician,
                Description = form.Description
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(AddAppointment), new {Id = appointment.Id}, appointment);
        }
    }
}