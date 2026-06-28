namespace CRM.Domain.Entities
{
    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Low, Medium, High, Critical
        public int Level { get; set; } // 1-4
        public string? ColorCode { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}