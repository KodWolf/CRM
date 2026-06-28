using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using System.Linq;

namespace CRM.Application.Services
{
	public class OperatorService : IOperatorService
	{
		public Task<OperatorDto?> GetByIdAsync(int id) => Task.FromResult<OperatorDto?>(null);
		public Task<OperatorDto?> GetByLoginAsync(string login) => Task.FromResult<OperatorDto?>(null);
		public Task<IEnumerable<OperatorDto>> GetAllAsync() => Task.FromResult(Enumerable.Empty<OperatorDto>());
		public Task<OperatorDto> RegisterAsync(RegisterDto dto) => Task.FromResult<OperatorDto>(default!);
		public Task<LoginResponseDto> LoginAsync(LoginDto dto) => Task.FromResult<LoginResponseDto>(default!);
		public Task<bool> ChangeRoleAsync(int id, string role) => Task.FromResult(false);
		public Task<bool> DeactivateAsync(int id) => Task.FromResult(false);
		public Task<bool> ActivateAsync(int id) => Task.FromResult(false);
	}
}
