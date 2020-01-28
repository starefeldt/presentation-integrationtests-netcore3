namespace StudentManagementApi.Domain.Models
{
    public enum MessageType
    {
        Registered
    }

    public class Message
    {
        public string Content { get; set; }
        public MessageType MessageType { get; set; }
    }
}
