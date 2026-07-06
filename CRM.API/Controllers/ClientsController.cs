using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ClientsController : ControllerBase
	{
		private readonly IClientService _clientService;

		public ClientsController(IClientService clientService)
		{
			_clientService = clientService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll()
		{
			var clients = await _clientService.GetAllAsync();
			return Ok(clients);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ClientDto>> GetById(int id)
		{
			var client = await _clientService.GetByIdAsync(id);
			if (client == null)
				return NotFound($"Клиент с ID {id} не найден");
			return Ok(client);
		}

		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<ClientDto>>> Search([FromQuery] string term)
		{
			if (string.IsNullOrEmpty(term))
				return BadRequest("Поисковый термин не может быть пустым");

			var clients = await _clientService.SearchAsync(term);
			return Ok(clients);
		}

		[HttpGet("iin/{iin}")]
		public async Task<ActionResult<ClientDto>> GetByIIN(string iin)
		{
			var client = await _clientService.GetByIINAsync(iin);
			if (client == null)
				return NotFound($"Клиент с ИИН {iin} не найден");
			return Ok(client);
		}

		[HttpPost]
		[Authorize(Roles = "Operator,SeniorOperator,Admin")]
		public async Task<ActionResult<ClientDto>> Create(CreateClientDto dto)
		{
			try
			{
				var client = await _clientService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Operator,SeniorOperator,Admin")]
		public async Task<ActionResult<ClientDto>> Update(int id, CreateClientDto dto)
		{
			var client = await _clientService.UpdateAsync(id, dto);
			if (client == null)
				return NotFound($"Клиент с ID {id} не найден");
			return Ok(client);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "SeniorOperator,Admin")]
		public async Task<ActionResult> Delete(int id)
		{
			var result = await _clientService.DeleteAsync(id);
			if (!result)
				return NotFound($"Клиент с ID {id} не найден");
			return NoContent();
		}
	}
}
