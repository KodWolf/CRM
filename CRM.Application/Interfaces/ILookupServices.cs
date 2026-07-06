using CRM.Application.DTOs;

namespace CRM.Application.Interfaces
{
    public interface IStatusService
    {
        Task<IEnumerable<StatusDto>> GetAllAsync();
        Task<StatusDto?> GetByIdAsync(int id);
        Task<StatusDto> CreateAsync(CreateStatusDto dto);
        Task<StatusDto?> UpdateAsync(int id, CreateStatusDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public interface IPriorityService
    {
        Task<IEnumerable<PriorityDto>> GetAllAsync();
        Task<PriorityDto?> GetByIdAsync(int id);
        Task<PriorityDto> CreateAsync(CreatePriorityDto dto);
        Task<PriorityDto?> UpdateAsync(int id, CreatePriorityDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public interface IChannelService
    {
        Task<IEnumerable<ChannelDto>> GetAllAsync();
        Task<ChannelDto?> GetByIdAsync(int id);
        Task<ChannelDto> CreateAsync(CreateChannelDto dto);
        Task<ChannelDto?> UpdateAsync(int id, CreateChannelDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public interface IAttachmentService
    {
        Task<IEnumerable<AttachmentDto>> GetByTicketIdAsync(int ticketId);
        Task<AttachmentDto?> GetByIdAsync(int id);
        Task<AttachmentDto> CreateAsync(CreateAttachmentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
