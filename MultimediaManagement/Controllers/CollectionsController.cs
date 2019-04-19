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
    public class CollectionsController : ControllerBase
    {
        public ICollectionRepository _collection;
        public IPlaceholderRepository _placeholder;


        public CollectionsController(ICollectionRepository collection, IPlaceholderRepository placeholder)
        {
            _collection = collection;
            _placeholder = placeholder;

        }

        // GET: api/Collections
        [HttpGet]
        public async Task<IEnumerable<Collection>> GetCollection()
        {
            using (_collection)
            {
                return await _collection.GetAll();
            }
        }

        // GET: api/Collections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Collection>> GetCollection([FromRoute] Guid id)
        {
            using (_collection)
            {
              
                var collection = await _collection.Get(id);

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
            using (_collection)
            {

                if (collection == null)
                {
                    return NotFound();
                }

                if (id != collection.Id)
                {
                    return BadRequest();
                }
                _collection.Update(collection);
                _collection.Commit();
                return Ok(collection);
            }
        }

        // POST: api/Collections
        [HttpPost]
        public ActionResult<Collection> PostCollection([FromBody] Collection collection)
        {
            using (_collection)
            {
                collection.Id = Guid.NewGuid();
                _collection.Add(collection);

                _collection.Commit();
                return Ok(collection);
            }
        }

        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Collection>> DeleteCollection([FromRoute] Guid id)
        {
            using (_collection)
            {
                var collection = await _collection.Get(id);
                _collection.Remove(collection);
                _collection.Commit();
                return Ok(collection);
            }
        }

        private bool CollectionExists([FromRoute] Guid id)
        {
            using (_collection)
            {
                return _collection.Any(e => e.Id == id);
            }
        }

        [HttpGet("public/{id}/{take}/{skip}")]
        public async Task<IActionResult> GetPublicCollections([FromRoute] Guid userId, [FromRoute] int take, [FromRoute] int skip)
        {

            using (_collection)
            {
                var collections = await _collection.Find(r => r.UserId != userId && r.Type == 0, skip, take);
                foreach (var collection in collections)
                {
                    collection.User = null;
                    collection.Placeholder = (ICollection<Placeholder>) await _placeholder.Find(r => r.CollectionId == collection.Id,0,10);
                    foreach (var placeholder in collection.Placeholder)
                    {
                        placeholder.Collection = null;
                    }
                }
                return Ok(collections);
            }
        }

        [HttpGet("public/{id}/{take}/{skip}/{keywords}")]
        public async Task<IActionResult> GetPublicCollectionsWithKeywords([FromRoute] Guid userId, [FromRoute] int take, [FromRoute] int skip, [FromRoute] String keywords)
        {
            using (_collection)
            {
                var keywordsArr = keywords.Split(',');
                var collections = await _collection.Find(r => r.UserId != userId && r.Type == 0 && keywordsArr.Any(el => r.Keywords.Contains(el)), skip, take);

                foreach (var collection in collections)
                {
                    collection.User = null;
                    collection.Placeholder = (ICollection<Placeholder>)await _placeholder.Find(r => r.CollectionId == collection.Id, 0, 10);
                    foreach (var placeholder in collection.Placeholder)
                    {
                        placeholder.Collection = null;
                    }
                }
                return Ok(collections);
            }
        }

        [HttpGet("{id}/{take}/{skip}")]
        public async Task<IActionResult> GetCollectionsForUser([FromRoute] Guid userId, [FromRoute] int take, [FromRoute] int skip)
        {
            using (_collection)
            {
                var collections = await _collection.Find(r => r.UserId == userId, skip, take);
                foreach (var collection in collections)
                {
                    collection.User = null;
                    collection.Placeholder = (ICollection<Placeholder>)await _collection.Find(r => r.CollectionId == collection.Id, 0, 10);
                    foreach (var placeholder in collection.Placeholder)
                    {
                        placeholder.Collection = null;
                    }
                }
                return Ok(collections);
            }
        }
    }
}
