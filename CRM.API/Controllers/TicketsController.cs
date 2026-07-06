using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace CRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAll()
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
                return NotFound($"Обращение с ID {id} не найдено");
            return Ok(ticket);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetByClient(int clientId)
        {
            var tickets = await _ticketService.GetByClientIdAsync(clientId);
            return Ok(tickets);
        }

        [HttpGet("status/{statusId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetByStatus(int statusId)
        {
            var tickets = await _ticketService.GetByStatusIdAsync(statusId);
            return Ok(tickets);
        }

        [HttpGet("operator/{operatorId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetByOperator(int operatorId)
        {
            var tickets = await _ticketService.GetByOperatorIdAsync(operatorId);
            return Ok(tickets);
        }

        [HttpPost]
        [Authorize(Roles = "Operator,SeniorOperator,Admin")]
        public async Task<ActionResult<TicketDto>> Create(CreateTicketDto dto)
        {
            try
            {
                var ticket = await _ticketService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Operator,SeniorOperator,Admin")]
        public async Task<ActionResult<TicketDto>> Update(int id, UpdateTicketDto dto)
        {
            var ticket = await _ticketService.UpdateAsync(id, dto);
            if (ticket == null)
                return NotFound($"Обращение с ID {id} не найдено");
            return Ok(ticket);
        }

        [HttpPatch("{id}/status/{statusId}")]
        [Authorize(Roles = "Operator,SeniorOperator,Admin")]
        public async Task<ActionResult<TicketDto>> ChangeStatus(int id, int statusId)
        {
            var operatorName = User.Identity?.Name ?? "System";
            var ticket = await _ticketService.ChangeStatusAsync(id, statusId, operatorName);
            if (ticket == null)
                return NotFound($"Обращение с ID {id} не найдено");
            return Ok(ticket);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SeniorOperator,Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _ticketService.DeleteAsync(id);
            if (!result)
                return NotFound($"Обращение с ID {id} не найдено");
            return NoContent();
        }
    }
}