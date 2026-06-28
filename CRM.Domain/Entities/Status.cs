namespace CRM.Domain.Entities
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // New, InProgress, WaitingClient, Resolved, Closed
        public string? Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}