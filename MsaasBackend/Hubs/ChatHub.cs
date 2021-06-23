using System;
using System.Linq;
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

        Task ReceiveVideoCallRequest(OutboundVideoCall request);

        Task ReceiveVideoCallResponse(OutboundVideoCall response);
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

        public async Task SendMessageToPhysician(InboundChatMessage message)
        {
            var userId = await GetPhysicianUserIdFromAppointment(message.AppointmentId);
            await Clients.User(Convert.ToString(userId))
                .ReceiveMessage(OutboundChatMessage.FromInbound(message));
        }

        [Authorize(Roles = "Physician")]
        public async Task SendMessageToUser(InboundChatMessage message)
        {
            var userId = await GetUserIdFromAppointment(message.AppointmentId);
            await Clients.User(Convert.ToString(userId))
                .ReceiveMessage(OutboundChatMessage.FromInbound(message));
        }

        public async Task SendVideoCallRequestToPhysician(InboundVideoCall request)
        {
            var userId = await GetPhysicianUserIdFromAppointment(request.AppointmentId);
            await Clients.User(Convert.ToString(userId))
                .ReceiveVideoCallRequest(OutboundVideoCall.FromInbound(request));
        }

        [Authorize(Roles = "Physician")]
        public async Task SendVideoCallResponseToUser(InboundVideoCall response)
        {
            var userId = await GetUserIdFromAppointment(response.AppointmentId);
            await Clients.User(Convert.ToString(userId))
                .ReceiveVideoCallResponse(OutboundVideoCall.FromInbound(response));
        }

        private int GetUserId()
        {
            var userId = Context.UserIdentifier;
            if (userId == null) throw new HubException("Unauthorized user");
            return Convert.ToInt32(userId);
        }

        private async Task<int> GetPhysicianUserIdFromAppointment(int appointmentId)
        {
            var appointments =
                from a in _context.Appointments
                    .Include(a => a.Physician)
                where a.Id == appointmentId && a.UserId == GetUserId()
                select a;
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) throw new HubException("Appointment not found");
            return appointment.Physician.UserId;
        }

        private async Task<int> GetUserIdFromAppointment(int appointmentId)
        {
            var appointments =
                from a in _context.Appointments
                    .Include(a => a.Physician)
                where a.Id == appointmentId && a.Physician.UserId == GetUserId()
                select a;
            var appointment = await appointments.FirstOrDefaultAsync();
            if (appointment == null) throw new HubException("Appointment not found");
            return appointment.UserId;
        }
    }
}