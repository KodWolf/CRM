using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;
using BC = BCrypt.Net.BCrypt;

namespace CRM.Application.Services
{
	public class OperatorService : IOperatorService
	{
		private readonly IGenericRepository<Operator> _operatorRepository;
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public OperatorService(
			IGenericRepository<Operator> operatorRepository,
			ApplicationDbContext context,
			IConfiguration configuration)
		{
			_operatorRepository = operatorRepository;
			_context = context;
			_configuration = configuration;
		}

		public async Task<IEnumerable<OperatorDto>> GetAllAsync()
		{
			var operators = await _operatorRepository.GetAllAsync();
			return operators.Select(MapToDto);
		}

		public async Task<OperatorDto?> GetByIdAsync(int id)
		{
			var op = await _operatorRepository.GetByIdAsync(id);
			return op == null ? null : MapToDto(op);
		}

		public async Task<OperatorDto?> GetByLoginAsync(string login)
		{
			var op = await _context.Operators
				.FirstOrDefaultAsync(o => o.Login == login);
			return op == null ? null : MapToDto(op);
		}

		public async Task<OperatorDto> RegisterAsync(RegisterDto dto)
		{
			// Проверяем, не существует ли уже оператор с таким логином
			var existingOperator = await _context.Operators
				.FirstOrDefaultAsync(o => o.Login == dto.Login);

			if (existingOperator != null)
				throw new ArgumentException($"Оператор с логином '{dto.Login}' уже существует");

			// Валидация роли
			var validRoles = new[] { "Operator", "SeniorOperator", "Admin" };
			if (!validRoles.Contains(dto.Role))
				throw new ArgumentException($"Неверная роль '{dto.Role}'");

			var passwordHash = BC.HashPassword(dto.Password);

			var op = new Operator
			{
				Login = dto.Login,
				PasswordHash = passwordHash,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				Role = dto.Role,
				Department = dto.Department,
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			await _operatorRepository.AddAsync(op);
			await _context.SaveChangesAsync();

			return MapToDto(op);
		}

		public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
		{
			var op = await _context.Operators
				.FirstOrDefaultAsync(o => o.Login == dto.Login && o.IsActive);

			if (op == null || !BC.Verify(dto.Password, op.PasswordHash))
				throw new UnauthorizedAccessException("Неверный логин или пароль");

			// Обновляем LastLoginAt
			op.LastLoginAt = DateTime.UtcNow;
			_operatorRepository.Update(op);
			await _context.SaveChangesAsync();

			var token = GenerateJwtToken(op);

			return new LoginResponseDto
			{
				Token = token,
				Operator = MapToDto(op)
			};
		}

		public async Task<bool> ChangeRoleAsync(int id, string role)
		{
			var validRoles = new[] { "Operator", "SeniorOperator", "Admin" };
			if (!validRoles.Contains(role))
				throw new ArgumentException($"Неверная роль '{role}'");

			var op = await _context.Operators.FirstOrDefaultAsync(o => o.Id == id);
			if (op == null)
				return false;

			op.Role = role;
			_operatorRepository.Update(op);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeactivateAsync(int id)
		{
			var op = await _context.Operators.FirstOrDefaultAsync(o => o.Id == id);
			if (op == null)
				return false;

			op.IsActive = false;
			_operatorRepository.Update(op);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ActivateAsync(int id)
		{
			var op = await _context.Operators.FirstOrDefaultAsync(o => o.Id == id);
			if (op == null)
				return false;

			op.IsActive = true;
			_operatorRepository.Update(op);
			await _context.SaveChangesAsync();
			return true;
		}

		private string GenerateJwtToken(Operator op)
		{
			var key = _configuration["Jwt:Key"] ?? "your-secret-key-here-min-16-chars";
			var issuer = _configuration["Jwt:Issuer"] ?? "CrmAPI";
			var audience = _configuration["Jwt:Audience"] ?? "CrmUsers";

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, op.Id.ToString()),
				new Claim(ClaimTypes.Name, op.Login),
				new Claim("FullName", $"{op.FirstName} {op.LastName}"),
				new Claim(ClaimTypes.Role, op.Role)
			};

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: DateTime.UtcNow.AddHours(8),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private OperatorDto MapToDto(Operator op)
		{
			return new OperatorDto
			{
				Id = op.Id,
				Login = op.Login,
				FirstName = op.FirstName,
				LastName = op.LastName,
				Role = op.Role,
				Department = op.Department,
				IsActive = op.IsActive,
				LastLoginAt = op.LastLoginAt
			};
		}
	}
}
