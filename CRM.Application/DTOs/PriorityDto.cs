namespace CRM.Application.DTOs
{
    public class PriorityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public string? ColorCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreatePriorityDto
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public string? ColorCode { get; set; }
    }
}
