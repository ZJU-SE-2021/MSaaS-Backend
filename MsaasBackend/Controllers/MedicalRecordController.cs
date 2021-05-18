using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MedicalRecordController
    {
        private readonly ILogger<MedicalRecordController> _logger;
        private readonly DataContext _context;

        public MedicalRecordController(ILogger<MedicalRecordController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, MedicalRecordForm form)
        {
            throw new NotImplementedException();
        }
    }
}