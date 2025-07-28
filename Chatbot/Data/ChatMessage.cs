namespace Chatbot.Data
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SessionId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Sender { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
