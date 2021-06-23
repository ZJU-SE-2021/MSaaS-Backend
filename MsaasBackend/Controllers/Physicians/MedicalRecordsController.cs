using System;
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
    public class MedicalRecordsController : Controller
    {
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(ILogger<MedicalRecordsController> logger, DataContext context) : base(context)
        {
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhysicianMedicalRecordById(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();

            var records =
                from r in _context.MedicalRecords
                    .Include(r => r.Appointment)
                    .ThenInclude(a => a.Physician)
                where r.Id == id && r.Appointment.Physician.UserId == userId
                select r;
            var record = await records.FirstOrDefaultAsync();
            if (record == null) return NotFound();

            return Ok(record.ToDto());
        }

        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status201Created)]
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
            return CreatedAtAction("GetMedicalRecordById", new {Id = medicalRecord.Id}, medicalRecord.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, MedicalRecordForm form)
        {
            throw new NotImplementedException();
        }
    }
}