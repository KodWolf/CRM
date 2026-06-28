using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using System.Linq;

namespace CRM.Application.Services
{
	public class MessageService : IMessageService
	{
		public Task<MessageDto> CreateAsync(CreateMessageDto dto) => Task.FromResult<MessageDto>(default!);
		public Task<bool> DeleteAsync(int id) => Task.FromResult(false);
		public Task<IEnumerable<MessageDto>> GetByTicketIdAsync(int ticketId) => Task.FromResult(Enumerable.Empty<MessageDto>());
	}
}
