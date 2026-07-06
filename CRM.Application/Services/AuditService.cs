using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories.Interfaces;

namespace CRM.Application.Services
{
	public class AuditService : IAuditService
	{
		private readonly IGenericRepository<TicketHistory> _auditRepository;
		private readonly ApplicationDbContext _context;

		public AuditService(IGenericRepository<TicketHistory> auditRepository, ApplicationDbContext context)
		{
			_auditRepository = auditRepository;
			_context = context;
		}

		public async Task LogChangeAsync(int ticketId, string fieldName, string? oldValue, string? newValue, string changedBy)
		{
			var history = new TicketHistory
			{
				TicketId = ticketId,
				FieldName = fieldName,
				OldValue = oldValue,
				NewValue = newValue,
				ChangedBy = changedBy,
				ChangedAt = DateTime.UtcNow
			};

			await _auditRepository.AddAsync(history);
			await _context.SaveChangesAsync();
		}
	}
}
