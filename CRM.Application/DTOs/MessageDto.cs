namespace CRM.Application.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsFromClient { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TicketId { get; set; }
        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; }
    }

    public class CreateMessageDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsFromClient { get; set; }
        public int TicketId { get; set; }
        public int? OperatorId { get; set; }
    }
}