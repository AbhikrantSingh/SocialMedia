using AbhiSocialApp.DBContext;
using AbhiSocialApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbhiSocialApp.Controllers
{
    [Route("messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly SocialAppDbContext _context;

        public MessagesController(SocialAppDbContext context)
        {
            _context = context;
        }

        // API to save a message
        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content))
            {
                return BadRequest("Invalid message data.");
            }

            message.Timestamp = DateTime.UtcNow; // Set the current timestamp
            message.IsSeen = false; // Mark as unseen by default

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok("Message saved successfully.");
        }
        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages(int senderId, int receiverId)
        {
            var messages = await (from message in _context.Messages
                                  join user in _context.Users
                                  on message.SenderId equals user.Id.ToString()
                                  where
                                  (message.SenderId == senderId.ToString() && message.ReceiverId == receiverId.ToString()) ||
                                  (message.SenderId == receiverId.ToString() && message.ReceiverId == senderId.ToString())
                                  select new
                                  {
                                      userName = user.UserName, // Sender's username
                                      id = message.Id,
                                      Timestamp = message.Timestamp,
                                      senderId = message.SenderId,
                                      receiverId = message.ReceiverId,
                                      content = message.Content
                                  }).ToListAsync();

            return Ok(messages);
        }


    }
}
