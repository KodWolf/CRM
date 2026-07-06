using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ChannelsController : ControllerBase
	{
		private readonly IChannelService _channelService;

		public ChannelsController(IChannelService channelService)
		{
			_channelService = channelService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ChannelDto>>> GetAll()
		{
			var channels = await _channelService.GetAllAsync();
			return Ok(channels);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ChannelDto>> GetById(int id)
		{
			var channel = await _channelService.GetByIdAsync(id);
			if (channel == null)
				return NotFound($"Канал с ID {id} не найден");
			return Ok(channel);
		}

		[HttpPost]
		[Authorize(Roles = "SeniorOperator,Admin")]
		public async Task<ActionResult<ChannelDto>> Create(CreateChannelDto dto)
		{
			try
			{
				var channel = await _channelService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = channel.Id }, channel);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "SeniorOperator,Admin")]
		public async Task<ActionResult<ChannelDto>> Update(int id, CreateChannelDto dto)
		{
			var channel = await _channelService.UpdateAsync(id, dto);
			if (channel == null)
				return NotFound($"Канал с ID {id} не найден");
			return Ok(channel);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> Delete(int id)
		{
			var result = await _channelService.DeleteAsync(id);
			if (!result)
				return NotFound($"Канал с ID {id} не найден");
			return NoContent();
		}
	}
}
