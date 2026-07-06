using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class AttachmentsController : ControllerBase
	{
		private readonly IAttachmentService _attachmentService;

		public AttachmentsController(IAttachmentService attachmentService)
		{
			_attachmentService = attachmentService;
		}

		[HttpGet("ticket/{ticketId}")]
		public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetByTicket(int ticketId)
		{
			var attachments = await _attachmentService.GetByTicketIdAsync(ticketId);
			return Ok(attachments);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AttachmentDto>> GetById(int id)
		{
			var attachment = await _attachmentService.GetByIdAsync(id);
			if (attachment == null)
				return NotFound($"Файл с ID {id} не найден");
			return Ok(attachment);
		}

		[HttpPost]
		[Authorize(Roles = "Operator,SeniorOperator,Admin")]
		public async Task<ActionResult<AttachmentDto>> Create(CreateAttachmentDto dto)
		{
			try
			{
				var attachment = await _attachmentService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = attachment.Id }, attachment);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "SeniorOperator,Admin")]
		public async Task<ActionResult> Delete(int id)
		{
			var result = await _attachmentService.DeleteAsync(id);
			if (!result)
				return NotFound($"Файл с ID {id} не найден");
			return NoContent();
		}
	}
}
