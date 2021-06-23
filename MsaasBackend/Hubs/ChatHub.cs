using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MsaasBackend.Controllers;
using MsaasBackend.Models;

namespace MsaasBackend.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(OutboundChatMessage message);
    }

    [Authorize(AuthenticationSchemes = AuthenticationDefaults.AuthenticationScheme)]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly DataContext _context;

        public ChatHub(ILogger<ChatHub> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "User")]
        public async Task SendMessageToPhysician(InboundChatMessage message)
        {
            var appointments =
                from a in _context.Appointments
                    .Include(a => a.Physician)
                where a.Id == message.AppointmentId && a.UserId == GetUserId()
                select a;
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) throw new HubException("Appointment not found");

            await Clients.User(Convert.ToString(appointment.Physician.UserId))
                .ReceiveMessage(OutboundChatMessage.FromInbound(message));
        }

        [Authorize(Roles = "Physician")]
        public async Task SendMessageToUser(InboundChatMessage message)
        {
            var appointments =
                from a in _context.Appointments
                    .Include(a => a.Physician)
                where a.Id == message.AppointmentId && a.Physician.UserId == GetUserId()
                select a;
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) throw new HubException("Appointment not found");
            await Clients.User(Convert.ToString(appointment.UserId))
                .ReceiveMessage(OutboundChatMessage.FromInbound(message));
        }

        private int GetUserId()
        {
            var userId = Context.UserIdentifier;
            if (userId == null) throw new HubException("Unauthorized user");
            return Convert.ToInt32(userId);
        }
    }
}