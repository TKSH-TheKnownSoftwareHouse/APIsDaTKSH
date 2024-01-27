using APIsDaTKSH.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APIsDaTKSH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepositionController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly ILogger<DepositionController> _logger;

        public DepositionController(MyDbContext dbContext, ILogger<DepositionController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DepositionModel deposition)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _dbContext.Depositions.Add(deposition);
                await _dbContext.SaveChangesAsync();

                return Ok("Deposition information added successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var depositions = _dbContext.Depositions.ToList();
                return Ok(depositions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var deposition = _dbContext.Depositions.FirstOrDefault(d => d.id == id);

                if (deposition == null)
                {
                    return NotFound($"Deposition with ID {id} not found.");
                }

                return Ok(deposition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeposition(int id, [FromBody] DepositionModel updatedDeposition)
        {
            try
            {
                var existingDeposition = _dbContext.Depositions.FirstOrDefault(d => d.id == id);

                if (existingDeposition == null)
                {
                    return NotFound($"Deposition with ID {id} not found.");
                }

                existingDeposition.Author = updatedDeposition.Author;
                existingDeposition.MainMessage = updatedDeposition.MainMessage;
                existingDeposition.Subtitle = updatedDeposition.Subtitle;
                existingDeposition.Rating = updatedDeposition.Rating;

                await _dbContext.SaveChangesAsync();

                return Ok($"Deposition with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeposition(int id)
        {
            try
            {
                var depositionToDelete = _dbContext.Depositions.FirstOrDefault(d => d.id == id);

                if (depositionToDelete == null)
                {
                    return NotFound($"Deposition with ID {id} not found.");
                }

                _dbContext.Depositions.Remove(depositionToDelete);
                await _dbContext.SaveChangesAsync();

                return Ok($"Deposition with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}
