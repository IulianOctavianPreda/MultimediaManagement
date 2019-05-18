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
        public async Task<ActionResult<Placeholder>> GetPlaceholderWithEntityFile([FromRoute] Guid id)
        {
            using (_placeholder)
            {
                var placeholder = await _placeholder.Get(id);

                if (placeholder == null)
                {
                    return NotFound();
                }

                var entityFile = _entityFile.FirstOrDefault(r => r.PlaceholderId == id);
                if (entityFile == null)
                {
                    return NotFound();
                }

                return placeholder;
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
                        }
                    }


                    _placeholder.Add(placeholder);
                }
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


        // GET: api/Collections/5
        [HttpGet("{id}/{take}/{skip}")]
        public async Task<IActionResult> GetPlaceholdersFromSpecificCollection([FromRoute] Guid collectionId, [FromRoute] int take, [FromRoute] int skip)
        {
            using (_collection)
            {

                var collection = await _collection.Get(collectionId);

                if (collection == null)
                {
                    return NotFound();
                }

                var placeHolders = await _placeholder.Find(r => r.CollectionId == collection.Id, skip, take);

                foreach (var placeHolder in placeHolders)
                {
                    placeHolder.Collection = null;
                }


                collection.Placeholder = (ICollection<Placeholder>)placeHolders;

                return Ok(collection);
            }
        }

        [HttpGet("{id}/{take}/{skip}/{keywords}")]
        public async Task<IActionResult> GetPlaceholdersFromSpecificCollectionWithKeywords([FromRoute] Guid collectionId, [FromRoute] int take, [FromRoute] int skip, [FromRoute] String keywords)
        {
            var keywordsArr = keywords.Split(',');

            var collection = await _collection.Get(collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var placeHolders = await _placeholder.Find(r => r.CollectionId != collectionId && keywordsArr.Any(el => r.Keywords != null && r.Keywords.Contains(el)), skip, take);

            foreach (var placeHolder in placeHolders)
            {
                placeHolder.Collection = null;
            }


            collection.Placeholder = (ICollection<Placeholder>)placeHolders;

            return Ok(collection);

        }
    }
}
