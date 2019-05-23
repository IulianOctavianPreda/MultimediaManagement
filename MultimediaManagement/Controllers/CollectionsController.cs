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
        public async Task<IEnumerable<Collection>> GetAllCollections()
        {
            using (_collection)
            {
                return await _collection.GetAll();
            }
        }

        // GET: api/Collections/5
        [HttpGet("{id}/{take}/{skip}")]
        public async Task<IActionResult> GetCollection([FromRoute] Guid id, [FromRoute] int take, [FromRoute] int skip)
        {
            using (_collection)
            {
                var collection = await _collection.FirstOrDefaultAsync(x => x.Id == id);

                if (collection == null)
                {
                    return NotFound();
                }

                var placeHolders = (ICollection<Placeholder>)await _placeholder.Find(r => r.CollectionId == collection.Id, skip, take);

                foreach (var placeHolder in placeHolders)
                {
                    placeHolder.Collection = null;
                }


                collection.Placeholder = placeHolders;

                return Ok(collection);
            }
        }

        // GET: api/Collections/5
        [HttpGet("{id}/{take}/{skip}/{keywords}")]
        public async Task<IActionResult> GetCollectionWithKeywords([FromRoute] Guid id, [FromRoute] int take, [FromRoute] int skip, [FromRoute] String keywords)
        {
            using (_collection)
            {
                var keywordsArr = keywords.Split(',');

                var placeHolders = (ICollection<Placeholder>)await _placeholder.Find(r => r.CollectionId != id && keywordsArr.Any(el => r.Keywords != null && r.Keywords.Contains(el)), skip, take);

                foreach (var placeHolder in placeHolders)
                {
                    placeHolder.Collection = null;
                }

                return Ok(placeHolders);
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
        public IActionResult PostCollection([FromBody] Collection collection)
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

    }
}
