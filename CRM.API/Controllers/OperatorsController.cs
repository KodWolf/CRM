using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace CRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperatorsController : ControllerBase
    {
        private readonly IOperatorService _operatorService;

        public OperatorsController(IOperatorService operatorService)
        {
            _operatorService = operatorService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OperatorDto>> Register(RegisterDto dto)
        {
            try
            {
                var result = await _operatorService.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
        {
            try
            {
                var result = await _operatorService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "SeniorOperator,Admin")]
        public async Task<ActionResult<IEnumerable<OperatorDto>>> GetAll()
        {
            var operators = await _operatorService.GetAllAsync();
            return Ok(operators);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SeniorOperator,Admin")]
        public async Task<ActionResult<OperatorDto>> GetById(int id)
        {
            var op = await _operatorService.GetByIdAsync(id);
            if (op == null)
                return NotFound($"Оператор с ID {id} не найден");
            return Ok(op);
        }

        [HttpPatch("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeRole(int id, [FromBody] string role)
        {
            var result = await _operatorService.ChangeRoleAsync(id, role);
            if (!result)
                return NotFound($"Оператор с ID {id} не найден");
            return Ok();
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Deactivate(int id)
        {
            var result = await _operatorService.DeactivateAsync(id);
            if (!result)
                return NotFound($"Оператор с ID {id} не найден");
            return Ok();
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Activate(int id)
        {
            var result = await _operatorService.ActivateAsync(id);
            if (!result)
                return NotFound($"Оператор с ID {id} не найден");
            return Ok();
        }
    }
}