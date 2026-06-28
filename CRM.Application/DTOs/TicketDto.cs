namespace CRM.Application.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public int ChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public List<MessageDto> Messages { get; set; } = new();
    }

    public class CreateTicketDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public int? OperatorId { get; set; }
        public int PriorityId { get; set; }
        public int ChannelId { get; set; }
    }

    public class UpdateTicketDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public int? OperatorId { get; set; }
    }
}
