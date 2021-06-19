using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MsaasBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly ILogger<MedicalRecordsController> _logger;
        private readonly DataContext _context;

        public MedicalRecordsController(ILogger<MedicalRecordsController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var records = 
                from r in _context.MedicalRecords.Include(r => r.Appointment)
                where r.Id == id
                select r;
            var record = await records.FirstOrDefaultAsync();
            if (record == null) return NotFound();

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();
            var userId = Convert.ToInt32(claim.Value);

            var appointment = record.Appointment;
            if (appointment.UserId != userId && appointment.Physician.UserId != userId) return Forbid();

            return Ok(record.ToDto());
        }

        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status201Created)]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> AddMedicalRecord(MedicalRecordForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var appointments =
                from a in _context.Appointments
                where a.Id == form.AppointmentId
                select a;
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) return NotFound();

            var medicalRecord = new MedicalRecord
            {
                AppointmentId = form.AppointmentId,
                Symptom = form.Symptom,
                PastMedicalHistory = form.Symptom,
                Diagnosis = form.Diagnosis
            };
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMedicalRecordById), new {Id = medicalRecord.Id}, medicalRecord);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, MedicalRecordForm form)
        {
            throw new NotImplementedException();
        }
    }
}