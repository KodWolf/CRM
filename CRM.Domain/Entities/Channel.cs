namespace CRM.Domain.Entities
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // CallCenter, Chat, Email, MobileApp, Website
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}