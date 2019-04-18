using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultimediaManagement.Models;
using MultimediaManagement.UoW;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceholdersController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        public PlaceholdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Placeholders
        [HttpGet]
        public async Task<IEnumerable<Placeholder>> GetPlaceholder()
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return await _unitOfWork.Placeholder.GetAll();
            }
        }

        // GET: api/Placeholders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Placeholder>> GetPlaceholder([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var placeholder = await _unitOfWork.Placeholder.Get(id);

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
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                if (placeholder == null)
                {
                    return NotFound();
                }

                if (id != placeholder.Id)
                {
                    return BadRequest();
                }
                _unitOfWork.Placeholder.Update(placeholder);
                _unitOfWork.Commit();
                return Ok(placeholder);
            }
        }

        // POST: api/Placeholders
        [HttpPost]
        public ActionResult<Placeholder> PostPlaceholder([FromBody] Placeholder placeholder)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                placeholder.Id = Guid.NewGuid();
                _unitOfWork.Placeholder.Add(placeholder);

                _unitOfWork.Commit();
                return Ok(placeholder);
            }
        }

        // DELETE: api/Placeholders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Placeholder>> DeletePlaceholder(Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var placeholder = await _unitOfWork.Placeholder.Get(id);
                _unitOfWork.Placeholder.Remove(placeholder);
                _unitOfWork.Commit();
                return Ok(placeholder);
            }
        }

        private bool PlaceholderExists(Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return _unitOfWork.Placeholder.Any(e => e.Id == id);
            }
        }
    }
}
