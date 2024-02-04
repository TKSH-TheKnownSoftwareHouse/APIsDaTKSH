// APIsDaTKSH.Controllers.HeroController
using APIsDaTKSH.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIsDaTKSH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeroController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly ILogger<HeroController> _logger;

        public HeroController(MyDbContext dbContext, ILogger<HeroController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] HeroModel hero)
        {
            try
            {
                var isAdminClaim = User.FindFirst("IsAdmin")?.Value;

                if (bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin)
                {

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    _dbContext.Heroes.Add(hero);
                    await _dbContext.SaveChangesAsync();

                    return Ok("Hero information added successfully!");
                }
                return Forbid();
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
                var heroes = _dbContext.Heroes.ToList();
                return Ok(heroes);
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
                var hero = _dbContext.Heroes.FirstOrDefault(h => h.id == id);

                if (hero == null)
                {
                    return NotFound($"Hero with ID {id} not found.");
                }

                return Ok(hero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateHero(int id, [FromBody] HeroModel updatedHero)
        {
            try
            {
                var isAdminClaim = User.FindFirst("IsAdmin")?.Value;

                if (bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin)
                {

                    var existingHero = _dbContext.Heroes.FirstOrDefault(h => h.id == id);

                    if (existingHero == null)
                    {
                        return NotFound($"Hero with ID {id} not found.");
                    }

                    existingHero.MainMessage = updatedHero.MainMessage;
                    existingHero.Subtitle = updatedHero.Subtitle;

                    await _dbContext.SaveChangesAsync();

                    return Ok($"Hero with ID {id} updated successfully.");
                }
                return Forbid();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteHero(int id)
        {
            try
            {
                var isAdminClaim = User.FindFirst("IsAdmin")?.Value;

                if (bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin) { 
                    var heroToDelete = _dbContext.Heroes.FirstOrDefault(h => h.id == id);

                if (heroToDelete == null)
                {
                    return NotFound($"Hero with ID {id} not found.");
                }

                _dbContext.Heroes.Remove(heroToDelete);
                await _dbContext.SaveChangesAsync();

                return Ok($"Hero with ID {id} deleted successfully.");
                }
                return Forbid();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}
