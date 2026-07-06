using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CRM.Application.Services
{
	public class MessageService : IMessageService
	{
		private readonly IGenericRepository<Message> _messageRepository;
		private readonly IGenericRepository<Ticket> _ticketRepository;
		private readonly IGenericRepository<Operator> _operatorRepository;
		private readonly ApplicationDbContext _context;

		public MessageService(
			IGenericRepository<Message> messageRepository,
			IGenericRepository<Ticket> ticketRepository,
			IGenericRepository<Operator> operatorRepository,
			ApplicationDbContext context)
		{
			_messageRepository = messageRepository;
			_ticketRepository = ticketRepository;
			_operatorRepository = operatorRepository;
			_context = context;
		}

		public async Task<IEnumerable<MessageDto>> GetByTicketIdAsync(int ticketId)
		{
			var messages = await _context.Messages
				.Include(m => m.Operator)
				.Where(m => m.TicketId == ticketId)
				.OrderBy(m => m.CreatedAt)
				.ToListAsync();

			return messages.Select(MapToDto);
		}

		public async Task<MessageDto> CreateAsync(CreateMessageDto dto)
		{
			// Проверяем существование обращения
			var ticket = await _ticketRepository.GetByIdAsync(dto.TicketId);
			if (ticket == null)
				throw new ArgumentException($"Обращение с ID {dto.TicketId} не найдено");

			// Проверяем оператора если не от клиента
			Operator? op = null;
			if (!dto.IsFromClient && dto.OperatorId.HasValue)
			{
				op = await _operatorRepository.GetByIdAsync(dto.OperatorId.Value);
				if (op == null)
					throw new ArgumentException($"Оператор с ID {dto.OperatorId} не найден");
			}

			var message = new Message
			{
				Text = dto.Text,
				IsFromClient = dto.IsFromClient,
				TicketId = dto.TicketId,
				OperatorId = dto.OperatorId,
				CreatedAt = DateTime.UtcNow
			};

			await _messageRepository.AddAsync(message);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(message.Id) ?? throw new InvalidOperationException("Не удалось создать сообщение");
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
			if (message == null)
				return false;

			_messageRepository.Delete(message);
			await _context.SaveChangesAsync();
			return true;
		}

		private async Task<MessageDto?> GetByIdAsync(int id)
		{
			var message = await _context.Messages
				.Include(m => m.Operator)
				.FirstOrDefaultAsync(m => m.Id == id);

			return message == null ? null : MapToDto(message);
		}

		private MessageDto MapToDto(Message message)
		{
			return new MessageDto
			{
				Id = message.Id,
				Text = message.Text,
				IsFromClient = message.IsFromClient,
				CreatedAt = message.CreatedAt,
				TicketId = message.TicketId,
				OperatorId = message.OperatorId,
				OperatorName = message.Operator == null ? null : $"{message.Operator.FirstName} {message.Operator.LastName}"
			};
		}
	}
}
