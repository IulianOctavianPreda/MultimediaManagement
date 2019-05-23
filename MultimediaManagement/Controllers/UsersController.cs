using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultimediaManagement.Models;
using MultimediaManagement.Repository;
using MultimediaManagement.Services;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _user;
        public ICollectionRepository _collection;
        public IPlaceholderRepository _placeholder;

        public UsersController(IUserRepository user, ICollectionRepository collection, IPlaceholderRepository placeholder)
        {
            _user = user;
            _collection = collection;
            _placeholder = placeholder;


        }


        // GET: api/Users
        [HttpGet]
        public async Task<IEnumerable<User>> GetUser()
        {
            using (_user)
            {
                return await _user.GetAll();
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser([FromRoute] Guid id)
        {
            using (_user)
            {
                var user = await _user.Get(id);

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public IActionResult PutUser([FromRoute] Guid id, [FromBody] User user)
        {
            using (_user)
            {

                if (user == null)
                {
                    return NotFound();
                }

                if (id != user.Id)
                {
                    return BadRequest();
                }
                _user.Update(user);
                _user.Commit();
                return Ok(user);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser([FromRoute] Guid id)
        {
            using (_user)
            {
                var user = await _user.Get(id);
                _user.Remove(user);
                _user.Commit();
                return Ok(user);
            }
        }

        private bool UserExists([FromRoute] Guid id)
        {
            using (_user)
            {
                return _user.Any(e => e.Id == id);
            }
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            using (_user)
            {
                
                var loginUser = await _user.FirstOrDefaultAsync(r => r.Username == user.Username);

                if (loginUser == null)
                {
                    return NotFound();
                }

                if (user.Username == "guest")
                {
                    loginUser.ModifiedOn = DateTime.Now;
                    _user.Update(loginUser);
                    _user.Commit();

                    return Ok(loginUser);
                }

                else
                {
                    var cryptoService = new CryptoService();
                    user.Password = cryptoService.Sha256_hash(user.Password);
             

                    if (user.Password != loginUser.Password)
                    {
                        return BadRequest();
                    }

                    loginUser.ModifiedOn = DateTime.Now;
                    loginUser.Token = Guid.NewGuid();
                    _user.Update(loginUser);
                    _user.Commit();
                    loginUser.Password = null;

                    return Ok(loginUser);
                }
                
            }
        }

 
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup([FromBody] User user)
        {
            using (_user)
            {
                var dbUser = await _user.FirstOrDefaultAsync(r => r.Username == user.Username);

                if (dbUser != null)
                {
                    return BadRequest();
                }
                else
                {
                    user.Id = Guid.NewGuid();

                    var cryptoService = new CryptoService();
                    user.Password = cryptoService.Sha256_hash(user.Password);
                    user.ModifiedOn = DateTime.Now;
                    user.Token = Guid.NewGuid();
           
                    _user.Add(user);
                    _user.Commit();

                    user.Password = null;

                    return Ok(user);
                }
                
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

        // GET: api/Users/5
        [HttpGet("{id}/{take}/{skip}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid userId, [FromRoute] int take, [FromRoute] int skip)
        {
            using (_user)
            {
                var user = await _user.FirstOrDefaultAsync(r => r.Id == userId);

                if (user == null)
                {
                    return NotFound();
                }
                user.Password = null;

                var collections = await _collection.Find(r => r.UserId == userId, skip, take);
                foreach (var collection in user.Collection)
                {
                    collection.User = null;
                    collection.Placeholder = (ICollection<Placeholder>)await _placeholder.Find(r => r.CollectionId == collection.Id, 0, 10);
                    foreach (var placeholder in collection.Placeholder)
                    {
                        placeholder.Collection = null;
                    }
                }
                return Ok(user);
            }
        }
    }
}
