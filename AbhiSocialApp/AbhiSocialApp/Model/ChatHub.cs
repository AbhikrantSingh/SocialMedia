using AbhiSocialApp.DBContext;
using AbhiSocialApp.Model;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace AbhiSocialApp.Model.DTO
{
    public class ChatHub : Hub
    {
        private readonly SocialAppDbContext _context;

        // Store user connections
        private static readonly ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        public ChatHub(SocialAppDbContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections[userId] = Context.ConnectionId;
                Console.WriteLine($"User {userId} connected with ConnectionId {Context.ConnectionId}");
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.TryRemove(userId, out _);
                Console.WriteLine($"User {userId} disconnected.");
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            Console.WriteLine($"SendMessage invoked. Sender: {senderId}, Receiver: {receiverId}, Message: {message}");

            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Invalid parameters provided.");
                throw new ArgumentException("Invalid parameters provided.");
            }

            try
            {
                // Save the message to the database
                var _message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = message,
                    Timestamp = DateTime.UtcNow,
                    IsSeen = false
                };

                _context.Messages.Add(_message);
                await _context.SaveChangesAsync();
                Console.WriteLine("Message saved to database successfully.");

                // Check if receiver is connected
                if (!UserConnections.TryGetValue(receiverId, out var connectionId))
                {
                    Console.WriteLine($"Receiver {receiverId} is not connected.");
                    return;
                }

                // Send the message in real time
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message, _message.Timestamp);
                Console.WriteLine("Message sent to receiver successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                throw;
            }
        }

    }
}
