using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CRM.Application.Services
{
	public class ClientService : IClientService
	{
		private readonly IGenericRepository<Client> _clientRepository;
		private readonly ApplicationDbContext _context;

		public ClientService(IGenericRepository<Client> clientRepository, ApplicationDbContext context)
		{
			_clientRepository = clientRepository;
			_context = context;
		}

		public async Task<IEnumerable<ClientDto>> GetAllAsync()
		{
			var clients = await _clientRepository.GetAllAsync();
			return clients.Select(MapToDto);
		}

		public async Task<ClientDto?> GetByIdAsync(int id)
		{
			var client = await _clientRepository.GetByIdAsync(id);
			return client == null ? null : MapToDto(client);
		}

		public async Task<ClientDto?> GetByIINAsync(string iin)
		{
			var client = await _context.Clients
				.FirstOrDefaultAsync(c => c.IIN == iin);
			return client == null ? null : MapToDto(client);
		}

		public async Task<IEnumerable<ClientDto>> SearchAsync(string searchTerm)
		{
			var clients = await _context.Clients
				.Where(c => c.FirstName.Contains(searchTerm) ||
						   c.LastName.Contains(searchTerm) ||
						   c.Email!.Contains(searchTerm) ||
						   c.PhoneNumber.Contains(searchTerm) ||
						   c.IIN.Contains(searchTerm))
				.ToListAsync();

			return clients.Select(MapToDto);
		}

		public async Task<ClientDto> CreateAsync(CreateClientDto dto)
		{
			// Проверяем уникальность ИИН
			var existingClient = await _context.Clients
				.FirstOrDefaultAsync(c => c.IIN == dto.IIN);

			if (existingClient != null)
				throw new ArgumentException($"Клиент с ИИН {dto.IIN} уже существует");

			var client = new Client
			{
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				MiddleName = dto.MiddleName,
				IIN = dto.IIN,
				PhoneNumber = dto.PhoneNumber,
				Email = dto.Email,
				Address = dto.Address,
				CreatedAt = DateTime.UtcNow,
				IsActive = true
			};

			await _clientRepository.AddAsync(client);
			await _context.SaveChangesAsync();

			return MapToDto(client);
		}

		public async Task<ClientDto?> UpdateAsync(int id, CreateClientDto dto)
		{
			var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
			if (client == null)
				return null;

			client.FirstName = dto.FirstName ?? client.FirstName;
			client.LastName = dto.LastName ?? client.LastName;
			client.MiddleName = dto.MiddleName ?? client.MiddleName;
			client.PhoneNumber = dto.PhoneNumber ?? client.PhoneNumber;
			client.Email = dto.Email ?? client.Email;
			client.Address = dto.Address ?? client.Address;

			_clientRepository.Update(client);
			await _context.SaveChangesAsync();

			return MapToDto(client);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
			if (client == null)
				return false;

			_clientRepository.Delete(client);
			await _context.SaveChangesAsync();
			return true;
		}

		private ClientDto MapToDto(Client client)
		{
			return new ClientDto
			{
				Id = client.Id,
				FirstName = client.FirstName,
				LastName = client.LastName,
				MiddleName = client.MiddleName,
				IIN = client.IIN,
				PhoneNumber = client.PhoneNumber,
				Email = client.Email,
				Address = client.Address,
				IsActive = client.IsActive
			};
		}
	}
}
