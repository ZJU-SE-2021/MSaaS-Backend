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

    public class RtcSessionDescription
    {
        public string Type { get; set; }

        public string Sdp { get; set; }
    }

    public class InboundVideoCall
    {
        public int AppointmentId { get; set; }

        public RtcSessionDescription Sdp { get; set; }
    }

    public class OutboundVideoCall : InboundVideoCall
    {
        public DateTime Time { get; set; }

        public static OutboundVideoCall FromInbound(InboundVideoCall request) => new()
        {
            AppointmentId = request.AppointmentId,
            Sdp = request.Sdp,
            Time = DateTime.Now
        };
    }
}