namespace CRM.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogChangeAsync(int ticketId, string fieldName, string? oldValue, string? newValue, string changedBy);
    }
}