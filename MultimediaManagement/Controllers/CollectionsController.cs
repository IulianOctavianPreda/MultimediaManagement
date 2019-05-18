using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultimediaManagement.Models;
using MultimediaManagement.Repository;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        public ICollectionRepository _collection;
        public IPlaceholderRepository _placeholder;
        private IUserRepository _user;

        public CollectionsController(ICollectionRepository collection, IPlaceholderRepository placeholder, IUserRepository user)
        {
            _collection = collection;
            _placeholder = placeholder;
            _user = user;

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
                IEnumerable<Collection> collections;
                if (_user.FirstOrDefault(x => x.Id == userId).Username == "guest")
                {
                    collections = await _collection.Find(r => r.UserId == userId && r.Type == 0, skip, take);
                    var collection1 = await _collection.Find(r => r.UserId != userId && r.Type == 0, skip, take);
                    collections = collections.Concat(collection1);
                }
                else
                {
                    collections = await _collection.Find(r => r.UserId != userId && r.Type == 0, skip, take);
                }
                foreach (var collection in collections)
                {
                    collection.User = null;
                    using (_placeholder)
                    {
                        collection.Placeholder = (ICollection<Placeholder>)await _placeholder.Find(r => r.CollectionId == collection.Id, 0, 10);
                        foreach (var placeholder in collection.Placeholder)
                        {
                            placeholder.Collection = null;
                        }
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
                IEnumerable<Collection> collections;
                if (_user.FirstOrDefault(x => x.Id == userId).Username == "guest")
                {
                    collections = await _collection.Find(r => r.UserId == userId && r.Type == 0 && keywordsArr.Any(el => r.Keywords.Contains(el)), skip, take);
                    var collection1 = await _collection.Find(r => r.UserId != userId && r.Type == 0 && keywordsArr.Any(el => r.Keywords.Contains(el)), skip, take);
                    collections = collections.Concat(collection1);
                }
                else
                {
                    collections = await _collection.Find(r => r.UserId != userId && r.Type == 0 && keywordsArr.Any(el => r.Keywords.Contains(el)), skip, take);
                }
                

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
                    collection.Placeholder = (ICollection<Placeholder>)await _placeholder.Find(r => r.CollectionId == collection.Id, 0, 10);
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
