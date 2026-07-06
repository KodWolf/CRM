using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class StatusesController : ControllerBase
	{
		private readonly IStatusService _statusService;

		public StatusesController(IStatusService statusService)
		{
			_statusService = statusService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<StatusDto>>> GetAll()
		{
			var statuses = await _statusService.GetAllAsync();
			return Ok(statuses);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<StatusDto>> GetById(int id)
		{
			var status = await _statusService.GetByIdAsync(id);
			if (status == null)
				return NotFound($"Статус с ID {id} не найден");
			return Ok(status);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<StatusDto>> Create(CreateStatusDto dto)
		{
			try
			{
				var status = await _statusService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<StatusDto>> Update(int id, CreateStatusDto dto)
		{
			var status = await _statusService.UpdateAsync(id, dto);
			if (status == null)
				return NotFound($"Статус с ID {id} не найден");
			return Ok(status);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> Delete(int id)
		{
			var result = await _statusService.DeleteAsync(id);
			if (!result)
				return NotFound($"Статус с ID {id} не найден");
			return NoContent();
		}
	}
}
