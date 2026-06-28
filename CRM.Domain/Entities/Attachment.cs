using System;

namespace CRM.Domain.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; } // в байтах
        public string FileType { get; set; } = string.Empty; // MIME type
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Внешние ключи
        public int TicketId { get; set; }
        public int? MessageId { get; set; }

        // Навигационные свойства
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual Message? Message { get; set; }
    }
}