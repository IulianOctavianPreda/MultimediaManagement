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
    public class CollectionsController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        public CollectionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Collections
        [HttpGet]
        public async Task<IEnumerable<Collection>> GetCollection()
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return await _unitOfWork.Collection.GetAll();
            }
        }

        // GET: api/Collections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Collection>> GetCollection([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var collection = await _unitOfWork.Collection.Get(id);

                if (collection == null)
                {
                    return NotFound();
                }

                return collection;
            }
        }

        // PUT: api/Collections/5
        [HttpPut("{id}")]
        public IActionResult PutCollection([FromRoute] Guid id, [FromBody] Collection collection)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                if (collection == null)
                {
                    return NotFound();
                }

                if (id != collection.Id)
                {
                    return BadRequest();
                }
                _unitOfWork.Collection.Update(collection);
                _unitOfWork.Commit();
                return Ok(collection);
            }
        }

        // POST: api/Collections
        [HttpPost]
        public ActionResult<Collection> PostCollection([FromBody] Collection collection)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();

                collection.Id = Guid.NewGuid();
                _unitOfWork.Collection.Add(collection);

                _unitOfWork.Commit();
                return Ok(collection);
            }
        }

        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Collection>> DeleteCollection([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                var collection = await _unitOfWork.Collection.Get(id);
                _unitOfWork.Collection.Remove(collection);
                _unitOfWork.Commit();
                return Ok(collection);
            }
        }

        private bool CollectionExists([FromRoute] Guid id)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Create();
                return _unitOfWork.Collection.Any(e => e.Id == id);
            }
        }
    }
}
