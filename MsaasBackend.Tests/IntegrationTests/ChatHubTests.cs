using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using MsaasBackend.Models;
using MsaasBackend.Tests.Utils;
using Xunit;

namespace MsaasBackend.Tests.IntegrationTests
{
    [Collection("Database collection")]
    public class ChatHubTests : TestBase
    {
        private HubConnection BuildConnection(LoginForm user)
        {
            return new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/hubs/chat",
                    o =>
                    {
                        o.AccessTokenProvider = async () => { return await GetToken(user); };
                        o.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                    })
                .Build();
        }

        public ChatHubTests(BackendFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task UserSendToPhysician_ValidIdentity_Success()
        {
            var userConn = BuildConnection(User);
            var physicianConn = BuildConnection(Physician);
            var mockHandler = new Mock<Action<OutboundChatMessage>>();

            physicianConn.On<OutboundChatMessage>("ReceiveMessage", mockHandler.Object);
            await physicianConn.StartAsync();
            await userConn.StartAsync();

            var message = new InboundChatMessage()
            {
                AppointmentId = 1,
                Message = "test message from user"
            };
            await userConn.SendAsync("SendMessageToPhysician", message);
            await mockHandler.VerifyWithTimeoutAsync(
                x => x(It.Is<OutboundChatMessage>(n => n.Message == message.Message)),
                Times.Once());
        }

        [Fact]
        public async Task PhysicianSendToUser_ValidIdentity_Success()
        {
            var physicianConn = BuildConnection(Physician);
            var userConn = BuildConnection(User);
            var mockHandler = new Mock<Action<OutboundChatMessage>>();

            userConn.On<OutboundChatMessage>("ReceiveMessage", mockHandler.Object);
            await userConn.StartAsync();
            await physicianConn.StartAsync();

            var message = new InboundChatMessage()
            {
                AppointmentId = 1,
                Message = "test message from physician"
            };
            await physicianConn.SendAsync("SendMessageToUser", message);
            await mockHandler.VerifyWithTimeoutAsync(
                x => x(It.Is<OutboundChatMessage>(n => n.Message == message.Message)),
                Times.Once());
        }

        [Fact]
        public async Task VideoCall_ValidIdentity_Success()
        {
            var physicianConn = BuildConnection(Physician);
            var userConn = BuildConnection(User);
            var userHandler = new Mock<Action<OutboundVideoCall>>();
            var physicianHandler = new Mock<Action<OutboundVideoCall>>();

            userConn.On<OutboundVideoCall>("ReceiveVideoCallResponse", userHandler.Object);
            physicianConn.On<OutboundVideoCall>("ReceiveVideoCallRequest", async req =>
            {
                var res = new InboundVideoCall()
                {
                    AppointmentId = req.AppointmentId,
                    Sdp = new RtcSessionDescription() {Type = "answer", Sdp = "xxx"}
                };

                await physicianConn.SendAsync("SendVideoCallResponseToUser", res);
            });
            await userConn.StartAsync();
            await physicianConn.StartAsync();

            var req = new InboundVideoCall()
            {
                AppointmentId = 1,
                Sdp = new RtcSessionDescription() {Type = "offer", Sdp = "xxx"}
            };

            await userConn.SendAsync("SendVideoCallRequestToPhysician", req);
            await userHandler.VerifyWithTimeoutAsync(
                x => x(It.Is<OutboundVideoCall>(n => n.Sdp.Type == "answer")),
                Times.Once());
        }
    }
}