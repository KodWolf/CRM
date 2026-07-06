using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class MessagesController : ControllerBase
	{
		private readonly IMessageService _messageService;

		public MessagesController(IMessageService messageService)
		{
			_messageService = messageService;
		}

		[HttpGet("ticket/{ticketId}")]
		public async Task<ActionResult<IEnumerable<MessageDto>>> GetByTicket(int ticketId)
		{
			var messages = await _messageService.GetByTicketIdAsync(ticketId);
			return Ok(messages);
		}

		[HttpPost]
		[Authorize(Roles = "Operator,SeniorOperator,Admin")]
		public async Task<ActionResult<MessageDto>> Create(CreateMessageDto dto)
		{
			try
			{
				var message = await _messageService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetByTicket), new { ticketId = message.TicketId }, message);
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

		[HttpDelete("{id}")]
		[Authorize(Roles = "SeniorOperator,Admin")]
		public async Task<ActionResult> Delete(int id)
		{
			var result = await _messageService.DeleteAsync(id);
			if (!result)
				return NotFound($"Сообщение с ID {id} не найдено");
			return NoContent();
		}
	}
}
