using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultimediaManagement.Models;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceholdersController : ControllerBase
    {
        private readonly MultimediaManagementContext _context;

        public PlaceholdersController(MultimediaManagementContext context)
        {
            _context = context;
        }

        // GET: api/Placeholders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Placeholder>>> GetPlaceholder()
        {
            return await _context.Placeholder.ToListAsync();
        }

        // GET: api/Placeholders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Placeholder>> GetPlaceholder(Guid id)
        {
            var placeholder = await _context.Placeholder.FindAsync(id);

            if (placeholder == null)
            {
                return NotFound();
            }

            return placeholder;
        }

        // PUT: api/Placeholders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaceholder(Guid id, Placeholder placeholder)
        {
            if (id != placeholder.Id)
            {
                return BadRequest();
            }

            _context.Entry(placeholder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceholderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Placeholders
        [HttpPost]
        public async Task<ActionResult<Placeholder>> PostPlaceholder(Placeholder placeholder)
        {
            _context.Placeholder.Add(placeholder);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PlaceholderExists(placeholder.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPlaceholder", new { id = placeholder.Id }, placeholder);
        }

        // DELETE: api/Placeholders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Placeholder>> DeletePlaceholder(Guid id)
        {
            var placeholder = await _context.Placeholder.FindAsync(id);
            if (placeholder == null)
            {
                return NotFound();
            }

            _context.Placeholder.Remove(placeholder);
            await _context.SaveChangesAsync();

            return placeholder;
        }

        private bool PlaceholderExists(Guid id)
        {
            return _context.Placeholder.Any(e => e.Id == id);
        }
    }
}
