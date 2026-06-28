using CRM.Application.Interfaces;

namespace CRM.Application.Services
{
	public class AuditService : IAuditService
	{
		public Task LogChangeAsync(int ticketId, string fieldName, string? oldValue, string? newValue, string changedBy)
		{
			return Task.CompletedTask;
		}
	}
}
