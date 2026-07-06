namespace CRM.Application.DTOs
{
    public class ChannelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateChannelDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
