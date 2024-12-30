using AbhiSocialApp.DBContext;
using AbhiSocialApp.Model;
using AbhiSocialApp.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbhiSocialApp.Controllers
{
    [Route("Post")]
    public class PostController : Controller
    {
        private readonly SocialAppDbContext _context;
        public PostController(SocialAppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllPost")]
        public async Task<IActionResult> GetAllPost()
        {
            var postDTOs = await (from post in _context.Posts
                                  join user in _context.Users on post.UserId equals user.Id
                                  select new PostDTO
                                  {
                                      Id = post.Id,
                                      Title = post.Title,
                                      Description = post.Description,
                                      UserName = user.UserName, // Get the username directly from User
                                      UserId = user.Id,
                                      ImagePath = post.ImagePath,
                                      VideoPath = post.VideoPath
                                  }).ToListAsync();

            return Ok(postDTOs);
        }



        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost([FromForm] IFormFile? image, [FromForm] IFormFile? video, [FromForm] string title, [FromForm] string description, [FromForm] int userId)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title is required.");
            }
            User _user = await _context.Users.FindAsync(userId);
            var post = new Post
            {
                Title = title,
                Description = description,
                UserId = userId,
                User = _user
            };

            // Save image locally if provided
            if (image != null)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images", image.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath)); // Ensure the directory exists
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                post.ImagePath = $"/uploads/images/{image.FileName}";
            }

            // Save video locally if provided
            if (video != null)
            {
                var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos", video.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(videoPath)); // Ensure the directory exists
                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
                post.VideoPath = $"/uploads/videos/{video.FileName}";
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post created successfully", PostId = post.Id });
        }

    }
}
