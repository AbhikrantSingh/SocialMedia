using AbhiSocialApp.DBContext;
using AbhiSocialApp.Model;
using AbhiSocialApp.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace AbhiSocialApp.Controllers
{
    [Route("Friendships")]
    public class FriendController : Controller
    {
        private readonly SocialAppDbContext _context;
        public FriendController(SocialAppDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            // Get the IDs of users who are already friends
            var friendIds = await _context.Friendships
                .Where(f => (f.UserId == userId && f.IsAccepted) || (f.FriendId == userId && f.IsAccepted))
                .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                .ToListAsync();

            // Get all users except the logged-in user and those already friends
            var users = await _context.Users
                .Where(u => u.Id != userId && !friendIds.Contains(u.Id))
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("SendFriendRequest")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto request)
        {
            if (request.SenderId <= 0 || request.ReceiverId <= 0)
            {
                return BadRequest("Invalid sender or receiver ID.");
            }
            // Check if a friend request already exists
            var existingRequest = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId == request.SenderId && f.FriendId == request.ReceiverId);

            if (existingRequest != null)
            {
                return BadRequest("Friend request already sent.");
            }

            var friendship = new Friendship
            {
                UserId = request.SenderId,
                FriendId = request.ReceiverId,
                CreatedAt = DateTime.UtcNow,
                IsAccepted = false // Initially false since the request is pending
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok("Friend request sent!");
        }

        [HttpPost("AcceptFriendRequest")]
        public async Task<IActionResult> AcceptFriendRequest([FromQuery] int friendshipId)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship == null)
                return BadRequest("No friendRequest exits");

            friendship.IsAccepted = true;
            await _context.SaveChangesAsync();

            return Ok("Friend Request Accepted");
        }
        [HttpGet("GetAllFriendRequestList")]
        public async Task<IActionResult> GetAllFriendRequestList(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            // Query pending friend requests
            var requestFriendList = await _context.Friendships
                .Where(f => f.FriendId == userId && !f.IsAccepted) // Pending requests
                .Select(f => new
                {
                    FriendshipId = f.Id, // ID of the friendship record
                    UserId = f.UserId, // ID of the user who sent the request
                    UserName = f.User.UserName, // Name of the user who sent the request
                    CreatedAt = f.CreatedAt // Timestamp of the request
                })
                .ToListAsync();

            if (requestFriendList.Count == 0)
                return Ok("No pending friend requests.");

            return Ok(requestFriendList);
        }

        [HttpGet("GetUserFriendList")]
        public async Task<IActionResult> GetUserFriendList(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid userId");

            // Fetch the user's friend list (bidirectional)
            var usersFriendList = await (from friendship in _context.Friendships
                                         from user in _context.Users
                                         where (friendship.UserId == userId && user.Id == friendship.FriendId) // Friends where the user initiated
                                            || (friendship.FriendId == userId && user.Id == friendship.UserId) // Friends where the user received
                                         select new
                                         {
                                             FriendId = user.Id,
                                             FriendName = user.UserName, // Assuming UserName is the desired field
                                             FriendEmail = user.Email    // Add any additional friend info you need
                                         }).ToListAsync();

            if (!usersFriendList.Any())
                return NotFound("No friends found for this user.");

            return Ok(usersFriendList);
        }
    }

}

