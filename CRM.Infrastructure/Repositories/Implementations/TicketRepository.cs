// TicketRepository.cs
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories.Implementations
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ticket>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.Operator)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Channel)
                .Include(t => t.Messages)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.Operator)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Channel)
                .Include(t => t.Messages)
                .Include(t => t.Histories)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Ticket>> GetByClientIdAsync(int clientId)
        {
            return await _dbSet
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByStatusIdAsync(int statusId)
        {
            return await _dbSet.Include(t => t.Client)
                .Include(t => t.Operator)
                .Where(t => t.StatusId == statusId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByOperatorIdAsync(int operatorId)
        {
            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.OperatorId == operatorId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(t => t.Client)
                .Include(t => t.Status)
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}