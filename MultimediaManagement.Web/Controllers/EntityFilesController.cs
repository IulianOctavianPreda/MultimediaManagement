using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultimediaManagement.Dao.Models;

namespace MultimediaManagement.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityFilesController : ControllerBase
    {
        private readonly MultimediaManagementContext _context;

        public EntityFilesController(MultimediaManagementContext context)
        {
            _context = context;
        }

        // GET: api/EntityFiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntityFile>>> GetEntityFile()
        {
            return await _context.EntityFile.ToListAsync();
        }

        // GET: api/EntityFiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityFile>> GetEntityFile(Guid id)
        {
            var entityFile = await _context.EntityFile.FindAsync(id);

            if (entityFile == null)
            {
                return NotFound();
            }

            return entityFile;
        }

        // PUT: api/EntityFiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntityFile(Guid id, EntityFile entityFile)
        {
            if (id != entityFile.Id)
            {
                return BadRequest();
            }

            _context.Entry(entityFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityFileExists(id))
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

        // POST: api/EntityFiles
        [HttpPost]
        public async Task<ActionResult<EntityFile>> PostEntityFile(EntityFile entityFile)
        {
            _context.EntityFile.Add(entityFile);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EntityFileExists(entityFile.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEntityFile", new { id = entityFile.Id }, entityFile);
        }

        // DELETE: api/EntityFiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EntityFile>> DeleteEntityFile(Guid id)
        {
            var entityFile = await _context.EntityFile.FindAsync(id);
            if (entityFile == null)
            {
                return NotFound();
            }

            _context.EntityFile.Remove(entityFile);
            await _context.SaveChangesAsync();

            return entityFile;
        }

        private bool EntityFileExists(Guid id)
        {
            return _context.EntityFile.Any(e => e.Id == id);
        }
    }
}
