using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CRM.Application.Services
{
	public class TicketService : ITicketService
	{
		private readonly IGenericRepository<Ticket> _ticketRepository;
		private readonly IGenericRepository<Client> _clientRepository;
		private readonly IGenericRepository<Operator> _operatorRepository;
		private readonly IGenericRepository<Status> _statusRepository;
		private readonly IGenericRepository<Priority> _priorityRepository;
		private readonly IGenericRepository<Channel> _channelRepository;
		private readonly IGenericRepository<Message> _messageRepository;
		private readonly ApplicationDbContext _context;
		private readonly IAuditService _auditService;

		public TicketService(
			IGenericRepository<Ticket> ticketRepository,
			IGenericRepository<Client> clientRepository,
			IGenericRepository<Operator> operatorRepository,
			IGenericRepository<Status> statusRepository,
			IGenericRepository<Priority> priorityRepository,
			IGenericRepository<Channel> channelRepository,
			IGenericRepository<Message> messageRepository,
			ApplicationDbContext context,
			IAuditService auditService)
		{
			_ticketRepository = ticketRepository;
			_clientRepository = clientRepository;
			_operatorRepository = operatorRepository;
			_statusRepository = statusRepository;
			_priorityRepository = priorityRepository;
			_channelRepository = channelRepository;
			_messageRepository = messageRepository;
			_context = context;
			_auditService = auditService;
		}

		public async Task<IEnumerable<TicketDto>> GetAllAsync()
		{
			var tickets = await _context.Tickets
				.Include(t => t.Client)
				.Include(t => t.Operator)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Include(t => t.Channel)
				.Include(t => t.Messages)
				.ToListAsync();

			return tickets.Select(MapToDto);
		}

		public async Task<TicketDto?> GetByIdAsync(int id)
		{
			var ticket = await _context.Tickets
				.Include(t => t.Client)
				.Include(t => t.Operator)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Include(t => t.Channel)
				.Include(t => t.Messages)
					.ThenInclude(m => m.Operator)
				.FirstOrDefaultAsync(t => t.Id == id);

			return ticket == null ? null : MapToDto(ticket);
		}

		public async Task<IEnumerable<TicketDto>> GetByClientIdAsync(int clientId)
		{
			var tickets = await _context.Tickets
				.Include(t => t.Client)
				.Include(t => t.Operator)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Include(t => t.Channel)
				.Include(t => t.Messages)
				.Where(t => t.ClientId == clientId)
				.ToListAsync();

			return tickets.Select(MapToDto);
		}

		public async Task<IEnumerable<TicketDto>> GetByOperatorIdAsync(int operatorId)
		{
			var tickets = await _context.Tickets
				.Include(t => t.Client)
				.Include(t => t.Operator)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Include(t => t.Channel)
				.Include(t => t.Messages)
				.Where(t => t.OperatorId == operatorId)
				.ToListAsync();

			return tickets.Select(MapToDto);
		}

		public async Task<IEnumerable<TicketDto>> GetByStatusIdAsync(int statusId)
		{
			var tickets = await _context.Tickets
				.Include(t => t.Client)
				.Include(t => t.Operator)
				.Include(t => t.Status)
				.Include(t => t.Priority)
				.Include(t => t.Channel)
				.Include(t => t.Messages)
				.Where(t => t.StatusId == statusId)
				.ToListAsync();

			return tickets.Select(MapToDto);
		}

		public async Task<TicketDto> CreateAsync(CreateTicketDto dto)
		{
			var client = await _clientRepository.GetByIdAsync(dto.ClientId);
			if (client == null)
				throw new ArgumentException($"Клиент с ID {dto.ClientId} не найден");

			var priority = await _priorityRepository.GetByIdAsync(dto.PriorityId);
			if (priority == null)
				throw new ArgumentException($"Приоритет с ID {dto.PriorityId} не найден");

			var channel = await _channelRepository.GetByIdAsync(dto.ChannelId);
			if (channel == null)
				throw new ArgumentException($"Канал с ID {dto.ChannelId} не найден");

			// Статус по умолчанию = "New" (ID = 1)
			var newStatus = await _statusRepository.GetByIdAsync(1);

			var ticket = new Ticket
			{
				Title = dto.Title,
				Description = dto.Description,
				ClientId = dto.ClientId,
				OperatorId = dto.OperatorId,
				StatusId = newStatus?.Id ?? 1,
				PriorityId = dto.PriorityId,
				ChannelId = dto.ChannelId,
				CreatedAt = DateTime.UtcNow
			};

			await _ticketRepository.AddAsync(ticket);
			await _context.SaveChangesAsync();

			// Логируем создание в аудит
			await _auditService.LogChangeAsync(ticket.Id, "Status", null, "New", "System");

			return await GetByIdAsync(ticket.Id) ?? throw new InvalidOperationException("Не удалось создать обращение");
		}

		public async Task<TicketDto?> UpdateAsync(int id, UpdateTicketDto dto)
		{
			var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
			if (ticket == null)
				return null;

			if (!string.IsNullOrEmpty(dto.Title) && ticket.Title != dto.Title)
			{
				await _auditService.LogChangeAsync(ticket.Id, "Title", ticket.Title, dto.Title, "System");
				ticket.Title = dto.Title;
			}

			if (!string.IsNullOrEmpty(dto.Description) && ticket.Description != dto.Description)
			{
				await _auditService.LogChangeAsync(ticket.Id, "Description", ticket.Description, dto.Description, "System");
				ticket.Description = dto.Description;
			}

			if (dto.PriorityId.HasValue && ticket.PriorityId != dto.PriorityId.Value)
			{
				var oldPriority = await _priorityRepository.GetByIdAsync(ticket.PriorityId);
				var newPriority = await _priorityRepository.GetByIdAsync(dto.PriorityId.Value);
				await _auditService.LogChangeAsync(ticket.Id, "Priority", oldPriority?.Name, newPriority?.Name, "System");
				ticket.PriorityId = dto.PriorityId.Value;
			}

			if (dto.OperatorId.HasValue && ticket.OperatorId != dto.OperatorId.Value)
			{
				var oldOperator = ticket.OperatorId.HasValue ? await _operatorRepository.GetByIdAsync(ticket.OperatorId.Value) : null;
				var newOperator = await _operatorRepository.GetByIdAsync(dto.OperatorId.Value);
				await _auditService.LogChangeAsync(ticket.Id, "Operator", oldOperator?.FirstName, newOperator?.FirstName, "System");
				ticket.OperatorId = dto.OperatorId.Value;
			}

			ticket.UpdatedAt = DateTime.UtcNow;
			_ticketRepository.Update(ticket);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(ticket.Id);
		}

		public async Task<TicketDto?> ChangeStatusAsync(int id, int statusId, string changedBy)
		{
			var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
			if (ticket == null)
				return null;

			var oldStatus = await _statusRepository.GetByIdAsync(ticket.StatusId);
			var newStatus = await _statusRepository.GetByIdAsync(statusId);

			if (newStatus == null)
				throw new ArgumentException($"Статус с ID {statusId} не найден");

			await _auditService.LogChangeAsync(ticket.Id, "Status", oldStatus?.Name, newStatus?.Name, changedBy);

			ticket.StatusId = statusId;
			ticket.UpdatedAt = DateTime.UtcNow;

			// Если статус = "Resolved" или "Closed", устанавливаем ClosedAt
			if (statusId == 4 || statusId == 5) // Resolved или Closed
			{
				ticket.ClosedAt = DateTime.UtcNow;
			}

			_ticketRepository.Update(ticket);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(ticket.Id);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
			if (ticket == null)
				return false;

			_ticketRepository.Delete(ticket);
			await _context.SaveChangesAsync();
			return true;
		}

		private TicketDto MapToDto(Ticket ticket)
		{
			return new TicketDto
			{
				Id = ticket.Id,
				Title = ticket.Title,
				Description = ticket.Description,
				CreatedAt = ticket.CreatedAt,
				ClosedAt = ticket.ClosedAt,
				ClientId = ticket.ClientId,
				ClientName = $"{ticket.Client?.FirstName} {ticket.Client?.LastName}",
				OperatorId = ticket.OperatorId,
				OperatorName = ticket.Operator == null ? null : $"{ticket.Operator.FirstName} {ticket.Operator.LastName}",
				StatusId = ticket.StatusId,
				StatusName = ticket.Status?.Name ?? "",
				PriorityId = ticket.PriorityId,
				PriorityName = ticket.Priority?.Name ?? "",
				ChannelId = ticket.ChannelId,
				ChannelName = ticket.Channel?.Name ?? "",
				Messages = ticket.Messages?.Select(m => new MessageDto
				{
					Id = m.Id,
					Text = m.Text,
					IsFromClient = m.IsFromClient,
					CreatedAt = m.CreatedAt,
					TicketId = m.TicketId,
					OperatorId = m.OperatorId,
					OperatorName = m.Operator == null ? null : $"{m.Operator.FirstName} {m.Operator.LastName}"
				}).ToList() ?? new List<MessageDto>()
			};
		}
	}
}
