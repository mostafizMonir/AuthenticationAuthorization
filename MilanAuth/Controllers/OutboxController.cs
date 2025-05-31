using Microsoft.AspNetCore.Mvc;
using MilanAuth.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using MilanAuth.Services;

namespace MilanAuth.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class OutboxController : ControllerBase
    {
        private readonly Repository<OutboxMessage> _repository;
        private readonly RabbitMQService  _mqService;

        public OutboxController(Repository<OutboxMessage> repository, RabbitMQService mqService)
        {
            _repository = repository;
            _mqService = mqService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OutboxMessage>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OutboxMessage>> GetById(int id)
        {
            var message = await _repository.GetByIdAsync(id);
            if (message == null) return NotFound();
            return Ok(message);
        }

        [HttpPost]
        public async Task<ActionResult<OutboxMessage>> Create(OutboxMessage message)
        {
            message.OccurredOn = DateTime.UtcNow;
            await _repository.AddAsync(message);
            return CreatedAtAction(nameof(GetById), new { id = message.Id }, message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OutboxMessage message)
        {
            if (id != message.Id) return BadRequest();
            await _repository.UpdateAsync(message);
            _mqService.PublishMessage();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _repository.GetByIdAsync(id);
            if (message == null) return NotFound();
            await _repository.DeleteAsync(message);
            return NoContent();
        }
    }

