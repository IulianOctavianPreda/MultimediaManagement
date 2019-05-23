using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultimediaManagement.Models;
using MultimediaManagement.Repository;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceholdersController : ControllerBase
    {
        private IPlaceholderRepository _placeholder;
        public ICollectionRepository _collection;
        private IEntityFileRepository _entityFile;

        public PlaceholdersController(IPlaceholderRepository placeholder, ICollectionRepository collection, IEntityFileRepository entityFile)
        {
            _placeholder = placeholder;
            _collection = collection;
            _entityFile = entityFile;
        }

        // GET: api/Placeholders
        [HttpGet]
        public async Task<IEnumerable<Placeholder>> GetPlaceholder()
        {
            using (_placeholder)
            {
                return await _placeholder.GetAll();
            }
        }

        // GET: api/Placeholders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityFile>> GetEntityFile([FromRoute] Guid id)
        {
            using (_entityFile)
            {
                var entityFile = await _entityFile.FirstOrDefaultAsync(r => r.PlaceholderId == id);
                if (entityFile == null)
                {
                    return NotFound();
                }

                return Ok(entityFile);
            }
        }

        //// PUT: api/Placeholders/5
        //[HttpPut("{id}")]
        //public  IActionResult PutPlaceholder([FromRoute] Guid id, [FromBody] Placeholder placeholder)
        //{
        //    using (_placeholder)
        //    {

        //        if (placeholder == null)
        //        {
        //            return NotFound();
        //        }

        //        if (id != placeholder.Id)
        //        {
        //            return BadRequest();
        //        }
        //        _placeholder.Update(placeholder);
        //        _placeholder.Commit();
        //        return Ok(placeholder);
        //    }
        //}

        // POST: api/Placeholders
        [HttpPost]
        public ActionResult<Placeholder> PostPlaceholderWithEntityFile([FromBody] List<Placeholder> placeholders)
        {
            using (_placeholder)
            {
                foreach (var placeholder in placeholders)
                {

                    placeholder.Id = Guid.NewGuid();
                    if (placeholder.EntityFile != null && placeholder.EntityFile.Count > 0)
                    {
                        foreach (var entityFile in placeholder.EntityFile)
                        {
                            entityFile.Id = Guid.NewGuid();
                            entityFile.PlaceholderId = placeholder.Id;
                           // _entityFile.Add(entityFile);
                        }

                    }


                    _placeholder.Add(placeholder);
                }
                _placeholder.Commit();
                foreach (var placeholder in placeholders)
                {
                    placeholder.Data = null;
                    placeholder.EntityFile = null;
                }
              
                return Ok(placeholders);
            }
        }

        // DELETE: api/Placeholders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Placeholder>> DeletePlaceholder(Guid id)
        {
            using (_placeholder)
            {
                var placeholder = await _placeholder.Get(id);
                _placeholder.Remove(placeholder);
                _placeholder.Commit();
                return Ok(placeholder);
            }
        }

        private bool PlaceholderExists(Guid id)
        {
            using (_placeholder)
            {
                return _placeholder.Any(e => e.Id == id);
            }
        }
        
    }
}
