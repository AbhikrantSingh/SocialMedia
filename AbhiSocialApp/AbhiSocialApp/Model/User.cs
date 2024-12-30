namespace AbhiSocialApp.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string phoneNumber { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Friendship> Friends { get; set; } = new List<Friendship>();

    }
}
