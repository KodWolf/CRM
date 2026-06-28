using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using System.Linq;

namespace CRM.Application.Services
{
	public class TicketService : ITicketService
	{
		public Task<TicketDto> CreateAsync(CreateTicketDto dto) => Task.FromResult<TicketDto>(default!);
		public Task<bool> DeleteAsync(int id) => Task.FromResult(false);
		public Task<TicketDto?> GetByIdAsync(int id) => Task.FromResult<TicketDto?>(null);
		public Task<IEnumerable<TicketDto>> GetAllAsync() => Task.FromResult(Enumerable.Empty<TicketDto>());
		public Task<IEnumerable<TicketDto>> GetByClientIdAsync(int clientId) => Task.FromResult(Enumerable.Empty<TicketDto>());
		public Task<IEnumerable<TicketDto>> GetByOperatorIdAsync(int operatorId) => Task.FromResult(Enumerable.Empty<TicketDto>());
		public Task<IEnumerable<TicketDto>> GetByStatusIdAsync(int statusId) => Task.FromResult(Enumerable.Empty<TicketDto>());
		public Task<TicketDto?> UpdateAsync(int id, UpdateTicketDto dto) => Task.FromResult<TicketDto?>(null);
		public Task<TicketDto?> ChangeStatusAsync(int id, int statusId, string changedBy) => Task.FromResult<TicketDto?>(null);
	}
}
