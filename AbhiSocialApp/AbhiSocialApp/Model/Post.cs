using AbhiSocialApp.Model;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ImagePath { get; set; } // Path to the saved image
    public string? VideoPath { get; set; } // Path to the saved video
    public int UserId { get; set; } // Foreign key for user association
    public User User { get; set; }
}
