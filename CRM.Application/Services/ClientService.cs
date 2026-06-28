using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using System.Linq;

namespace CRM.Application.Services
{
	public class ClientService : IClientService
	{
		public Task<ClientDto> CreateAsync(CreateClientDto dto) => Task.FromResult<ClientDto>(default!);
		public Task<bool> DeleteAsync(int id) => Task.FromResult(false);
		public Task<IEnumerable<ClientDto>> GetAllAsync() => Task.FromResult(Enumerable.Empty<ClientDto>());
		public Task<ClientDto?> GetByIdAsync(int id) => Task.FromResult<ClientDto?>(null);
		public Task<ClientDto?> GetByIINAsync(string iin) => Task.FromResult<ClientDto?>(null);
		public Task<IEnumerable<ClientDto>> SearchAsync(string searchTerm) => Task.FromResult(Enumerable.Empty<ClientDto>());
		public Task<ClientDto?> UpdateAsync(int id, CreateClientDto dto) => Task.FromResult<ClientDto?>(null);
	}
}
