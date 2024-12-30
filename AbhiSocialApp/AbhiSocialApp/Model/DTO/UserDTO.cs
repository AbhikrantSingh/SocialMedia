namespace AbhiSocialApp.Model.DTO
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string phoneNumber { get; set; }
        public IFormFile ProfilePhoto { get; set; }
    }
}
