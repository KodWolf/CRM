using CRM.Domain.Entities;

namespace CRM.Infrastructure.Repositories.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetAllWithDetailsAsync();
        Task<Ticket?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Ticket>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<Ticket>> GetByStatusIdAsync(int statusId);
        Task<IEnumerable<Ticket>> GetByOperatorIdAsync(int operatorId);
        Task<IEnumerable<Ticket>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}