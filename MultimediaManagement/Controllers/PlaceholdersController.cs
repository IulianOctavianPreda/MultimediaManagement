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

        public PlaceholdersController(IPlaceholderRepository placeholder)
        {
            _placeholder = placeholder;
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
        public async Task<ActionResult<Placeholder>> GetPlaceholder([FromRoute] Guid id)
        {
            using (_placeholder)
            {
                var placeholder = await _placeholder.Get(id);

                if (placeholder == null)
                {
                    return NotFound();
                }

                return placeholder;
            }
        }

        // PUT: api/Placeholders/5
        [HttpPut("{id}")]
        public  IActionResult PutPlaceholder([FromRoute] Guid id, [FromBody] Placeholder placeholder)
        {
            using (_placeholder)
            {

                if (placeholder == null)
                {
                    return NotFound();
                }

                if (id != placeholder.Id)
                {
                    return BadRequest();
                }
                _placeholder.Update(placeholder);
                _placeholder.Commit();
                return Ok(placeholder);
            }
        }

        // POST: api/Placeholders
        [HttpPost]
        public ActionResult<Placeholder> PostPlaceholder([FromBody] Placeholder placeholder)
        {
            using (_placeholder)
            {

                placeholder.Id = Guid.NewGuid();
                _placeholder.Add(placeholder);

                _placeholder.Commit();
                return Ok(placeholder);
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
