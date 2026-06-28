// IOperatorService.cs
using CRM.Application.DTOs;

namespace CRM.Application.Interfaces
{
    public interface IOperatorService
    {
        Task<IEnumerable<OperatorDto>> GetAllAsync();
        Task<OperatorDto?> GetByIdAsync(int id);
        Task<OperatorDto?> GetByLoginAsync(string login);
        Task<OperatorDto> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<bool> ChangeRoleAsync(int id, string role);
        Task<bool> DeactivateAsync(int id);
        Task<bool> ActivateAsync(int id);
    }
}