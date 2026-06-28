using CRM.Application.DTOs;

namespace CRM.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(int id);
        Task<ClientDto?> GetByIINAsync(string iin);
        Task<IEnumerable<ClientDto>> SearchAsync(string searchTerm);
        Task<ClientDto> CreateAsync(CreateClientDto dto);
        Task<ClientDto?> UpdateAsync(int id, CreateClientDto dto);
        Task<bool> DeleteAsync(int id);
    }
}