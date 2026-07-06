using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PrioritiesController : ControllerBase
	{
		private readonly IPriorityService _priorityService;

		public PrioritiesController(IPriorityService priorityService)
		{
			_priorityService = priorityService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<PriorityDto>>> GetAll()
		{
			var priorities = await _priorityService.GetAllAsync();
			return Ok(priorities);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<PriorityDto>> GetById(int id)
		{
			var priority = await _priorityService.GetByIdAsync(id);
			if (priority == null)
				return NotFound($"Приоритет с ID {id} не найден");
			return Ok(priority);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<PriorityDto>> Create(CreatePriorityDto dto)
		{
			try
			{
				var priority = await _priorityService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = priority.Id }, priority);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<PriorityDto>> Update(int id, CreatePriorityDto dto)
		{
			var priority = await _priorityService.UpdateAsync(id, dto);
			if (priority == null)
				return NotFound($"Приоритет с ID {id} не найден");
			return Ok(priority);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> Delete(int id)
		{
			var result = await _priorityService.DeleteAsync(id);
			if (!result)
				return NotFound($"Приоритет с ID {id} не найден");
			return NoContent();
		}
	}
}
