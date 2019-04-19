using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultimediaManagement.Models;
using MultimediaManagement.Repository;
using MultimediaManagement.UoW;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityFilesController : ControllerBase
    {
        private IEntityFileRepository _entityFile;

        public EntityFilesController(IEntityFileRepository entityFile)
        {
            _entityFile = entityFile;
        }

        // GET: api/EntityFiles
        [HttpGet]
        public async Task<IEnumerable<EntityFile>> GetEntityFile()
        {
            using (_entityFile)
            {
                return await _entityFile.GetAll();
            }
        }

        // GET: api/EntityFiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityFile>> GetEntityFile([FromRoute] Guid id)
        {
            using (_entityFile)
            {
                var entityFile = await _entityFile.Get(id);

                if (entityFile == null)
                {
                    return NotFound();
                }

                return entityFile;
            }
        }

        // PUT: api/EntityFiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntityFile([FromRoute] Guid id, [FromBody] EntityFile entityFile)
        {
            using (_entityFile)
            {
                if (entityFile == null)
                {
                    return NotFound();
                }

                if (id != entityFile.Id)
                {
                    return BadRequest();
                }
                _entityFile.Update(entityFile);
                _entityFile.Commit();
                return Ok(entityFile);
            }
        }

        // POST: api/EntityFiles
        [HttpPost]
        public ActionResult<EntityFile> PostEntityFile([FromBody] EntityFile entityFile)
        {
            using (_entityFile)
            {

                entityFile.Id = Guid.NewGuid();
                _entityFile.Add(entityFile);

                _entityFile.Commit();
                return Ok(entityFile);
            }
        }

        // DELETE: api/EntityFiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EntityFile>> DeleteEntityFile([FromRoute] Guid id)
        {
            using (_entityFile)
            {
                var entityFile = await _entityFile.Get(id);
                _entityFile.Remove(entityFile);
                _entityFile.Commit();
                return Ok(entityFile);
            }
        }

        private bool EntityFileExists([FromRoute] Guid id)
        {
            using (_entityFile)
            {
                return _entityFile.Any(e => e.Id == id);
            }
        }
    }
}
