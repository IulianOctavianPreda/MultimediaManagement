using System;
using System.Collections.Generic;
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
                    _user.Update(user);
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
    }
}
