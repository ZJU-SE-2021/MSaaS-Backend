using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MsaasBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly ILogger<MedicalRecordController> _logger;
        private readonly DataContext _context;

        public MedicalRecordController(ILogger<MedicalRecordController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecord), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddMedicalRecord(MedicalRecordForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var appointments = from a in _context.Appointments where a.Id == form.AppointmentId select a;
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) return NotFound();

            var medicalRecord = new MedicalRecord
            {
                AppointmentId = form.AppointmentId,
                Appointment = appointment,
                Symptom = form.Symptom,
                PastMedicalHistory = form.Symptom,
                Diagnosis = form.Diagnosis
            };
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(AddMedicalRecord), new {Id = medicalRecord.Id}, medicalRecord);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, MedicalRecordForm form)
        {
            throw new NotImplementedException();
        }
    }
}