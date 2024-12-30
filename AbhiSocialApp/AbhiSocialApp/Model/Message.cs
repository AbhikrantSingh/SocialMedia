namespace AbhiSocialApp.Model
{
    public class Message
    {
        public int Id { get; set; } // Primary Key
        public string SenderId { get; set; } // User ID of the sender
        public string ReceiverId { get; set; } // User ID of the receiver
        public string Content { get; set; } // The message content
        public DateTime Timestamp { get; set; } // When the message was sent
        public bool IsSeen { get; set; } // Whether the message was seen by the receiver
    }
}
