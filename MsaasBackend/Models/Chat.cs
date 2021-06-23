using System;

namespace MsaasBackend.Models
{
    public class InboundChatMessage
    {
        public int AppointmentId { get; set; }
        public string Message { get; set; }
    }

    public class OutboundChatMessage : InboundChatMessage
    {
        public DateTime Time { get; set; }
        
        public static OutboundChatMessage FromInbound(InboundChatMessage message) => new()
        {
            AppointmentId = message.AppointmentId,
            Message = message.Message,
            Time = DateTime.Now
        };
    }
}