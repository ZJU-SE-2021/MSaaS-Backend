using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MsaasBackend.Models;

namespace MsaasBackend.Controllers
{
    public class Controller : ControllerBase
    {
        protected readonly DataContext _context;

        public Controller(DataContext context)
        {
            _context = context;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public int? GetCurrentUserId()
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentId == null) return null;
            return Convert.ToInt32(currentId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<User> GetCurrentUser()
        {
            var id = GetCurrentUserId();
            if (!id.HasValue) return null;
            return await _context.Users.FindAsync(id);
        }
    }
}