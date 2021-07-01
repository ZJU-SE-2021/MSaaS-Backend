using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MsaasBackend.Helpers;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly DataContext _context;
        private readonly IDistributedCache _distributedCache;

        public DepartmentsController(ILogger<DepartmentsController> logger, DataContext context, IDistributedCache distributedCache)
        {
            _logger = logger;
            _context = context;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartments(int? hospitalId)
        {
            var cacheKey = "Departments";
            if (hospitalId.HasValue)
            {
                cacheKey += $"?HospitalId={hospitalId}";
            }
            var cachedDepartments = await _distributedCache.GetStringAsync(cacheKey);
            if (cachedDepartments != null && cachedDepartments != "[]")
            {
                return Ok(JsonSerializer.Deserialize<DepartmentDto[]>(cachedDepartments));
            }
            
            var departments =
                from d in _context.Departments
                    .Include(d => d.Hospital)
                where !hospitalId.HasValue || d.HospitalId == hospitalId
                select d.ToDto();
            var departmentsList = await departments.ToListAsync();
            var serializedString = JsonSerializer.Serialize(departmentsList);
            var option =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(2));
            await _distributedCache.SetStringAsync(cacheKey, serializedString, option);
            return Ok(departmentsList);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var cachedDepartment = await _distributedCache.GetStringAsync($"Departments/{id}");
            if (cachedDepartment != null && cachedDepartment != "{}")
            {
                return Ok(JsonSerializer.Deserialize<DepartmentDto>(cachedDepartment));
            }
            
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            await _context.Entry(department).Reference(d => d.Hospital).LoadAsync();
            var departmentDto = department.ToDto();
            var serializedString = JsonSerializer.Serialize(departmentDto);
            var option =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddHours(2));
            await _distributedCache.SetStringAsync($"Departments/{id}", serializedString, option);
            return Ok(departmentDto);
        }
    }
}