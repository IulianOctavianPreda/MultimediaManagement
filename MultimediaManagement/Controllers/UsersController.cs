using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultimediaManagement.Models;
using MultimediaManagement.Repository;
using MultimediaManagement.Services;
using MultimediaManagement.UoW;

namespace MultimediaManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _user;

        public UsersController(IUserRepository user)
        {
            _user = user;
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

        // POST: api/Users
        [HttpPost]
        public ActionResult<User> PostUser([FromBody] User user)
        {
            using (_user)
            {
                user.Id = Guid.NewGuid();
                var cryproService = new CryptoService();
                user.Password = cryproService.Sha256_hash(user.Password);

                _user.Add(user);

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

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            using (_user)
            {
                var loginUser = await _user.FirstOrDefaultAsync(r => r.Username == user.Username);

                if (loginUser == null)
                {
                    return NotFound();
                }
                var cryptoService = new CryptoService();
                user.Password = cryptoService.Sha256_hash(user.Password);
                if (user.Password != loginUser.Password)
                {
                    return BadRequest();
                }
                loginUser.Password = null;
                return Ok(loginUser);
            }
        }
    }
}
