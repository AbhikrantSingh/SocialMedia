namespace AbhiSocialApp.Model
{
    public class Friendship
    {
        public int Id { get; set; } // Primary key
        public int UserId { get; set; } // The user who initiated the friendship
        public User User { get; set; } // Navigation property for the first user

        public int FriendId { get; set; } // The friend being added
        public User Friend { get; set; } // Navigation property for the friend

        public DateTime CreatedAt { get; set; } // Timestamp for when the friendship was created
        public bool IsAccepted { get; set; } // Indicates if the friendship is accepted
    }
}
