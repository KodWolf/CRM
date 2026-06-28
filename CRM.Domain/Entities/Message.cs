using System;

namespace CRM.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsFromClient { get; set; } // true - от клиента, false - от оператора
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Внешние ключи
        public int TicketId { get; set; }
        public int? OperatorId { get; set; }

        // Навигационные свойства
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual Operator? Operator { get; set; }
    }
}