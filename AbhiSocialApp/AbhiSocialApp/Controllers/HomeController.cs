using AbhiSocialApp.DBContext;
using AbhiSocialApp.Model;
using AbhiSocialApp.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("Home")]
public class HomeController : Controller
{
    private readonly SocialAppDbContext _context;

    public HomeController(SocialAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("Login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        Console.WriteLine($"Email: {email}, Password: {password}");
        if (email == null || password == null)
        {
            return BadRequest("Please share email and password");

        }
        User? user =await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        if(user != null) 
        { 
            return Ok(new
            {
                Message = "Login successful",
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.phoneNumber,
                    user.UserName
                }
            });
        }
        else
        {
            return BadRequest("Password or email is wrong");
        }
    }

    [HttpPost("Add")]
    public async Task<IActionResult> AddUser([FromForm] UserDTO userDto)
    {
        if (_context.Users.Any(u => u.UserName == userDto.UserName))
        {
            return BadRequest("Username already exists.");
        }

        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            Password = userDto.Password,
            UserName = userDto.UserName,
            phoneNumber = userDto.phoneNumber
        };

        if (userDto.ProfilePhoto != null)
        {
            var uploadsFolder = Path.Combine("wwwroot/uploads/profile-photos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + userDto.ProfilePhoto.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await userDto.ProfilePhoto.CopyToAsync(stream);
            }

            user.ProfilePhotoUrl = $"/uploads/profile-photos/{uniqueFileName}";
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User added successfully" });
    }


}
