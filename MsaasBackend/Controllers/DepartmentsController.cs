using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Models;

namespace Msaasbackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly DataContext _context;

        public DepartmentsController(ILogger<DepartmentsController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpDelete("{id:int}")]
        public Task<IActionResult> DeleteDepartment(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id:int}")]
        public Task<IActionResult> UpdateDepartment(int id, DepartmentCreationForm form)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id:int}/Physicians")]
        public async Task<IActionResult> RegisterPhysician(int id, PhysicianRegisterForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var departments = from d in _context.Departments where d.Id == id select d;
            var department = await departments.FirstOrDefaultAsync();
            if (department == null) return NotFound();

            var physicians = from u in _context.Physicians where u.Id == form.PhysicianId select u;
            var physician = await physicians.FirstOrDefaultAsync();
            if (physician == null) return NotFound();

            physician.DepartmentId = id;
            physician.Department = department;

            return Ok(physician);
        }
    }
}