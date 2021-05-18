using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPatch("{id:int}")]
        public Task<IActionResult> UpdateDepartment(int id, DepartmentCreationForm form)
        {
            throw new NotImplementedException();
        }
    }
}