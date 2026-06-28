using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CRM.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ticket
            modelBuilder.Entity<Ticket>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(2000);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Client)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Operator)
                .WithMany(o => o.Tickets)
                .HasForeignKey(t => t.OperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Status)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Priority)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Channel)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Client
            modelBuilder.Entity<Client>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Client>()
                .Property(c => c.IIN)
                .IsRequired()
                .HasMaxLength(12);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.IIN)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            // Operator
            modelBuilder.Entity<Operator>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Operator>()
                .Property(o => o.Login)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Operator>()
                .HasIndex(o => o.Login)
                .IsUnique();

            modelBuilder.Entity<Operator>()
                .Property(o => o.PasswordHash)
                .IsRequired();

            // Message
            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .Property(m => m.Text)
                .IsRequired()
                .HasMaxLength(2000);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Operator)
                .WithMany(o => o.Messages)
                .HasForeignKey(m => m.OperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Attachment
            modelBuilder.Entity<Attachment>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Attachment>()
                .Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Status
            modelBuilder.Entity<Status>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Status>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Priority
            modelBuilder.Entity<Priority>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Priority>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Channel
            modelBuilder.Entity<Channel>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Channel>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            // TicketHistory
            modelBuilder.Entity<TicketHistory>()
                .HasKey(th => th.Id);

            modelBuilder.Entity<TicketHistory>()
                .Property(th => th.FieldName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<TicketHistory>()
                .Property(th => th.ChangedBy)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<TicketHistory>()
                .HasOne(th => th.Ticket)
                .WithMany(t => t.Histories)
                .HasForeignKey(th => th.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed данных
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Статусы
            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = "New", Description = "Новое обращение", Order = 1 },
                new Status { Id = 2, Name = "InProgress", Description = "В работе", Order = 2 },
                new Status { Id = 3, Name = "WaitingClient", Description = "Ожидание клиента", Order = 3 },
                new Status { Id = 4, Name = "Resolved", Description = "Решено", Order = 4 },
                new Status { Id = 5, Name = "Closed", Description = "Закрыто", Order = 5 }
            );

            // Приоритеты
            modelBuilder.Entity<Priority>().HasData(
                new Priority { Id = 1, Name = "Low", Level = 1, ColorCode = "#4CAF50" },
                new Priority { Id = 2, Name = "Medium", Level = 2, ColorCode = "#FFC107" },
                new Priority { Id = 3, Name = "High", Level = 3, ColorCode = "#FF9800" },
                new Priority { Id = 4, Name = "Critical", Level = 4, ColorCode = "#F44336" }
            );

            // Каналы
            modelBuilder.Entity<Channel>().HasData(
                new Channel { Id = 1, Name = "CallCenter", Description = "Колл-центр" },
                new Channel { Id = 2, Name = "Chat", Description = "Чат на сайте" },
                new Channel { Id = 3, Name = "Email", Description = "Электронная почта" },
                new Channel { Id = 4, Name = "MobileApp", Description = "Мобильное приложение" },
                new Channel { Id = 5, Name = "Website", Description = "Веб-сайт" }
            );

            // Тестовые клиенты
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, FirstName = "Иван", LastName = "Петров", MiddleName = "Сергеевич", IIN = "900101300001", PhoneNumber = "+77012345678", Email = "ivan@mail.com", Address = "г. Алматы, ул. Абая, 10" }
            );
        }
    }
}