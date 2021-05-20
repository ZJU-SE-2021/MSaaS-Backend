using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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


        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = from d in _context.Departments select d.ToDto();
            return Ok(await departments.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            return Ok(department.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var department = await _context.Departments.FindAsync(id);

            if (department.Name != form.Name)
            {
                var departments = from d in _context.Departments where d.Name == form.Name select d;
                if (await departments.AnyAsync()) return Conflict();
                department.Name = form.Name;
            }

            await _context.SaveChangesAsync();

            return Ok(department);
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> CreateDepartment(int id, DepartmentCreationForm form)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null) return NotFound();

            var departments =
                from d in _context.Departments
                where d.HospitalId == id && d.Name == form.Name
                select d;
            if (await departments.AnyAsync()) return Conflict();

            var department = new Department
            {
                Name = form.Name,
                //Hospital = hospital,
                HospitalId = id
            };

            //hospital.Departments.Add(department);
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { Id = department.Id }, department.ToDto());
        }
    }
}