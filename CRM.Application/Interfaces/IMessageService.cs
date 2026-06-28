// IMessageService.cs
using CRM.Application.DTOs;

namespace CRM.Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetByTicketIdAsync(int ticketId);
        Task<MessageDto> CreateAsync(CreateMessageDto dto);
        Task<bool> DeleteAsync(int id);
    }
}