using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.MiddleWare
{
    public class CommunicationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserService _userService; 
        
        public CommunicationMiddleware(RequestDelegate next, IUserService userService)
        {
            _next = next;
            _userService = userService;
        }
        
        public async Task Invoke(HttpContext context) 
        { 
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var ct = context.RequestAborted;
                var json = await RecieveStringAsync(webSocket, ct);
                var command = JsonConvert.DeserializeObject<dynamic>(json);

                switch (command.Operation.ToString())
                {
                    case "CheckEmailConfirmationStatus":
                    {
                        await ProccessEmailConfirmation(context, webSocket, ct, command.Parameters.ToString());
                        break;
                    }
                }
                
            } 
            else if (context.Request.Path.Equals("/CheckEmailConfirmationStatus"))
            {
                { 
                    await _next?.Invoke(context); 
                } 
            }
        }

        public async Task ProccessEmailConfirmation(HttpContext context, WebSocket currentSocket, CancellationToken ct,
            string email)
        {
            UserModel user = await _userService.GetUserByEmail(email);

            while (!ct.IsCancellationRequested && !currentSocket.CloseStatus.HasValue && user?.IsEmailConfirmed == false)
            {
                if (user.IsEmailConfirmed)
                {
                    await SendStringAsync(currentSocket, "OK", ct);
                }
                else
                {
                    user.IsEmailConfirmed = true;
                    user.EmailConfirmationDate = DateTime.Now;
                    await _userService.UpdateUser(user);
                    await SendStringAsync(currentSocket, "OK", ct);
                }

                Task.Delay(500).Wait();
                user = await _userService.GetUserByEmail(email);
            }
        }
        
        private static Task SendStringAsync(WebSocket socket, string data,
            CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> RecieveStringAsync(WebSocket socket,
            CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();
                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                    
                } while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    throw new Exception("Unexpected Message");
                }

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}