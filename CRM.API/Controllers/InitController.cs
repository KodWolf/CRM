using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InitController : ControllerBase
	{
		private readonly IOperatorService _operatorService;
		private readonly IClientService _clientService;
		private readonly IConfiguration _configuration;
		private readonly ILogger<InitController> _logger;

		public InitController(
			IOperatorService operatorService,
			IClientService clientService,
			IConfiguration configuration,
			ILogger<InitController> logger)
		{
			_operatorService = operatorService;
			_clientService = clientService;
			_configuration = configuration;
			_logger = logger;
		}

		/// <summary>
		/// Инициализирует систему, создавая администратора по умолчанию.
		/// </summary>
		[HttpPost("setup")]
		public async Task<ActionResult<object>> Setup()
		{
			var initPassword = _configuration["Init:AdminPassword"] ?? "admin123";
			var initLogin = _configuration["Init:AdminLogin"] ?? "admin";

			try
			{
				// Проверяем существует ли уже администратор
				var existingAdmin = await _operatorService.GetByLoginAsync(initLogin);
				if (existingAdmin != null)
				{
					return BadRequest(new { message = "Администратор уже создан в системе" });
				}

				// Создаем администратора
				var adminRegisterDto = new RegisterDto
				{
					Login = initLogin,
					Password = initPassword,
					FirstName = "Admin",
					LastName = "System",
					Department = "Administration",
					Role = "Admin"
				};

				var admin = await _operatorService.RegisterAsync(adminRegisterDto);

				_logger.LogInformation($"Администратор {initLogin} успешно создан");

				return Ok(new
				{
					message = "Система инициализирована успешно",
					admin = admin,
					credentials = new
					{
						login = initLogin,
						password = "***СКРЫТ***",
						note = "Пожалуйста, смените пароль администратора после первого входа"
					}
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при инициализации системы");
				return StatusCode(500, new { message = "Ошибка при инициализации", error = ex.Message });
			}
		}

		/// <summary>
		/// Проверяет статус инициализации системы.
		/// </summary>
		[HttpGet("status")]
		public async Task<ActionResult<object>> Status()
		{
			try
			{
				var adminExists = await _operatorService.GetByLoginAsync("admin") != null;
				return Ok(new
				{
					initialized = adminExists,
					status = adminExists ? "Система готова к работе" : "Требуется инициализация (вызовите POST /api/init/setup)"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Ошибка при проверке статуса", error = ex.Message });
			}
		}
	}
}
