using CRM.Application.DTOs;

namespace CRM.Application.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllAsync();
        Task<TicketDto?> GetByIdAsync(int id);
        Task<IEnumerable<TicketDto>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<TicketDto>> GetByStatusIdAsync(int statusId);
        Task<IEnumerable<TicketDto>> GetByOperatorIdAsync(int operatorId);
        Task<TicketDto> CreateAsync(CreateTicketDto dto);
        Task<TicketDto?> UpdateAsync(int id, UpdateTicketDto dto);
        Task<bool> DeleteAsync(int id);
        Task<TicketDto?> ChangeStatusAsync(int id, int statusId, string changedBy);
    }
}