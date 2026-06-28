using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading.Channels;

namespace CRM.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Внешние ключи
        public int ClientId { get; set; }
        public int? OperatorId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public int ChannelId { get; set; }

        // Навигационные свойства
        public virtual Client Client { get; set; } = null!;
        public virtual Operator? Operator { get; set; }
        public virtual Status Status { get; set; } = null!;
        public virtual Priority Priority { get; set; } = null!;
        public virtual Channel Channel { get; set; } = null!;

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public virtual ICollection<TicketHistory> Histories { get; set; } = new List<TicketHistory>();
    }
}